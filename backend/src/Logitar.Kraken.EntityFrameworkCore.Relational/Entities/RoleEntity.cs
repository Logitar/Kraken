using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Roles.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class RoleEntity : AggregateEntity
{
  public int RoleId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }

  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? CustomAttributes { get; private set; }

  public List<ApiKeyEntity> ApiKeys { get; private set; } = [];
  public List<UserEntity> Users { get; private set; } = [];

  public RoleEntity(RealmEntity? realm, RoleCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;

    RoleId roleId = new(@event.StreamId);
    Id = roleId.EntityId;

    UniqueName = @event.UniqueName.Value;
  }

  private RoleEntity() : base()
  {
  }

  public void SetUniqueName(RoleUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(RoleUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
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

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
