using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles.Events;

namespace Logitar.Kraken.Core.Roles;

internal class RoleManager : IRoleManager
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public RoleManager(IApplicationContext applicationContext, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<IReadOnlyDictionary<string, Role>> FindAsync(IEnumerable<string> values, CancellationToken cancellationToken)
  {
    RealmId? realmId = _applicationContext.RealmId;

    int capacity = values.Count();
    Dictionary<string, Role> foundRoles = new(capacity);
    HashSet<string> missingRoles = new(capacity);

    HashSet<RoleId> ids = new(capacity);
    HashSet<string> uniqueNames = new(capacity);
    foreach (string value in values)
    {
      if (Guid.TryParse(value.Trim(), out Guid entityId))
      {
        ids.Add(new RoleId(entityId, realmId));
      }
      else
      {
        uniqueNames.Add(value);
      }
    }

    if (uniqueNames.Count > 0)
    {
      IReadOnlyCollection<RoleId> roleIds = await _roleQuerier.FindIdsAsync(uniqueNames, cancellationToken);
      ids.AddRange(roleIds);
    }

    IReadOnlyCollection<Role> roles = ids.Count > 0 ? await _roleRepository.LoadAsync(ids, cancellationToken) : [];
    Dictionary<Guid, Role> rolesById = new(capacity: roles.Count);
    Dictionary<string, Role> rolesByUniqueName = new(capacity: roles.Count);
    foreach (Role role in roles)
    {
      rolesById[role.EntityId] = role;
      rolesByUniqueName[role.UniqueName.Value.ToUpperInvariant()] = role;
    }

    foreach (string value in values)
    {
      Role? role;
      if (Guid.TryParse(value.Trim(), out Guid entityId))
      {
        _ = rolesById.TryGetValue(entityId, out role);
      }
      else
      {
        _ = rolesByUniqueName.TryGetValue(value.Trim().ToUpperInvariant(), out role);
      }

      if (role == null)
      {
        missingRoles.Add(value);
      }
      else
      {
        foundRoles[value] = role;
      }
    }

    if (missingRoles.Count > 0)
    {
      throw new RolesNotFoundException(missingRoles);
    }

    return foundRoles.AsReadOnly();
  }

  public async Task SaveAsync(Role role, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = role.Changes.Any(change => change is RoleCreated || change is RoleUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      RoleId? conflictId = await _roleQuerier.FindIdAsync(role.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(role.Id))
      {
        throw new UniqueNameAlreadyUsedException(role, conflictId.Value);
      }
    }

    await _roleRepository.SaveAsync(role, cancellationToken);
  }
}
