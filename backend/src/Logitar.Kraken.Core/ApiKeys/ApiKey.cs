using Logitar.EventSourcing;
using Logitar.Kraken.Core.ApiKeys.Events;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;

namespace Logitar.Kraken.Core.ApiKeys;

public class ApiKey : AggregateRoot // TODO(fpion): tests
{
  public new ApiKeyId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private Password? _secret = null;

  private DisplayName? _name = null;
  public DisplayName Name => _name ?? throw new InvalidOperationException("The API key has not been initialized.");

  private readonly HashSet<RoleId> _roles = [];
  public IReadOnlyCollection<RoleId> Roles => _roles.ToList().AsReadOnly();

  public ApiKey() : base()
  {
  }

  public ApiKey(Password secret, DisplayName name, ActorId? actorId = null, ApiKeyId? apiKeyId = null)
    : base(apiKeyId?.StreamId)
  {
    Raise(new ApiKeyCreated(secret, name), actorId);
  }
  protected virtual void Handle(ApiKeyCreated @event)
  {
    _secret = @event.Secret;

    _name = @event.Name;
  }

  public void AddRole(Role role, ActorId? actorId = null)
  {
    // TODO(fpion): ensure is in same realm

    if (!HasRole(role))
    {
      Raise(new ApiKeyRoleAdded(role.Id), actorId);
    }
  }
  protected virtual void Handle(ApiKeyRoleAdded @event)
  {
    _roles.Add(@event.RoleId);
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ApiKeyDeleted(), actorId);
    }
  }

  public bool HasRole(Role role) => HasRole(role.Id);
  public bool HasRole(RoleId roleId) => _roles.Contains(roleId);

  public void RemoveRole(Role role, ActorId? actorId = null) // TODO(fpion): unit tests
  {
    if (HasRole(role))
    {
      Raise(new ApiKeyRoleRemoved(role.Id), actorId);
    }
  }
  public void RemoveRole(RoleId roleId, ActorId? actorId = null)
  {
    if (HasRole(roleId))
    {
      Raise(new ApiKeyRoleRemoved(roleId), actorId);
    }
  }
  protected virtual void Handle(ApiKeyRoleRemoved @event)
  {
    _roles.Remove(@event.RoleId);
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
