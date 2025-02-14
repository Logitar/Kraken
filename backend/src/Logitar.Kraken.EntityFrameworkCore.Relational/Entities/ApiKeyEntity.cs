using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.Core.ApiKeys.Events;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class ApiKeyEntity : AggregateEntity, ISegregatedEntity
{
  public int ApiKeyId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public string SecretHash { get; private set; } = string.Empty;

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }
  public DateTime? ExpiresOn { get; private set; }

  public DateTime? AuthenticatedOn { get; private set; }

  public string? CustomAttributes { get; private set; }

  public List<RoleEntity> Roles { get; private set; } = [];

  public ApiKeyEntity(RealmEntity? realm, ApiKeyCreated @event) : base(@event)
  {
    if (realm != null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

    ApiKeyId apiKeyId = new(@event.StreamId);
    Id = apiKeyId.EntityId;

    SecretHash = @event.Secret.Encode();

    Name = @event.Name.Value;
  }

  private ApiKeyEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipRoles: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipRoles)
  {
    List<ActorId> actorIds = [];
    actorIds.AddRange(base.GetActorIds());

    if (!skipRoles)
    {
      foreach (RoleEntity role in Roles)
      {
        actorIds.AddRange(role.GetActorIds());
      }
    }

    return actorIds.AsReadOnly();
  }

  public void AddRole(RoleEntity role, ApiKeyRoleAdded @event)
  {
    Update(@event);

    Roles.Add(role);
  }

  public void Authenticate(ApiKeyAuthenticated @event)
  {
    Update(@event);

    AuthenticatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void RemoveRole(ApiKeyRoleRemoved @event)
  {
    Update(@event);

    RoleEntity? role = Roles.SingleOrDefault(x => x.StreamId == @event.RoleId.StreamId.Value);
    if (role != null)
    {
      Roles.Remove(role);
    }
  }

  public void Update(ApiKeyUpdated @event)
  {
    base.Update(@event);

    if (@event.Name != null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
    if (@event.ExpiresOn.HasValue)
    {
      ExpiresOn = @event.ExpiresOn.Value.AsUniversalTime();
    }

    Dictionary<string, string> customAttributes = GetCustomAttributes();
    foreach (KeyValuePair<Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        customAttributes.Remove(customAttribute.Key.Value);
      }
      else
      {
        customAttributes[customAttribute.Key.Value] = customAttribute.Value;
      }
    }
    SetCustomAttributes(customAttributes);
  }

  public Dictionary<string, string> GetCustomAttributes()
  {
    return (CustomAttributes == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(CustomAttributes)) ?? [];
  }
  private void SetCustomAttributes(Dictionary<string, string> customAttributes)
  {
    CustomAttributes = customAttributes.Count < 1 ? null : JsonSerializer.Serialize(customAttributes);
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
