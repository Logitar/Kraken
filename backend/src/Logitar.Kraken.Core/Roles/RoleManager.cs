using Logitar.Kraken.Core.Roles.Events;

namespace Logitar.Kraken.Core.Roles;

internal class RoleManager : IRoleManager
{
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public RoleManager(IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
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
