using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Passwords.Events;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class OneTimePasswordEntity : AggregateEntity
{
  public int OneTimePasswordId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public UserEntity? User { get; private set; }
  public int? UserId { get; private set; }

  public Guid Id { get; private set; }

  public string PasswordHash { get; private set; } = string.Empty;

  public DateTime? ExpiresOn { get; private set; }
  public int? MaximumAttempts { get; private set; }

  public int AttemptCount { get; private set; }
  public bool HasValidationSucceeded { get; private set; }

  public string? CustomAttributes { get; private set; }

  public OneTimePasswordEntity(RealmEntity? realm, OneTimePasswordCreated @event) : this(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
  }
  public OneTimePasswordEntity(UserEntity user, OneTimePasswordCreated @event) : this(@event)
  {
    Realm = user.Realm;
    RealmId = user.RealmId;
  }
  private OneTimePasswordEntity(OneTimePasswordCreated @event) : base(@event)
  {
    OneTimePasswordId oneTimePasswordId = new(@event.StreamId);
    Id = oneTimePasswordId.EntityId;

    PasswordHash = @event.Password.Encode();

    ExpiresOn = @event.ExpiresOn?.AsUniversalTime();
    MaximumAttempts = @event.MaximumAttempts;
  }

  private OneTimePasswordEntity() : base()
  {
  }

  public void Fail(OneTimePasswordValidationFailed @event)
  {
    Update(@event);

    AttemptCount++;
  }

  public void Succeed(OneTimePasswordValidationSucceeded @event)
  {
    Update(@event);

    AttemptCount++;
    HasValidationSucceeded = true;
  }

  public void Update(OneTimePasswordUpdated @event)
  {
    base.Update(@event);

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
}
