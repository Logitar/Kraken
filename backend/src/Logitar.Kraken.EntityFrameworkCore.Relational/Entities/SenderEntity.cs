using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Senders.Events;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class SenderEntity : AggregateEntity
{
  public int SenderId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public Guid Id { get; private set; }

  public SenderType Type { get; private set; }
  public bool IsDefault { get; private set; }

  public string? EmailAddress { get; private set; }
  public string? PhoneNumber { get; private set; }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public SenderProvider Provider { get; private set; }
  public string? Settings { get; private set; }

  public List<MessageEntity> Messages { get; private set; } = [];

  public SenderEntity(RealmEntity? realm, SenderCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;

    SenderId senderId = new(@event.StreamId);
    Id = senderId.EntityId;

    if (@event.Email != null)
    {
      EmailAddress = @event.Email.Address;
    }
    if (@event.Phone != null)
    {
      PhoneNumber = @event.Phone.Number;
    }

    Provider = @event.Provider;
  }

  private SenderEntity() : base()
  {
  }

  public void SetDefault(SenderSetDefault @event)
  {
    Update(@event);

    IsDefault = @event.IsDefault;
  }

  public void SetMailgunSettings(SenderMailgunSettingsChanged @event)
  {
    Update(@event);

    SetSettings(new Dictionary<string, string>
    {
      [nameof(IMailgunSettings.ApiKey)] = @event.Settings.ApiKey,
      [nameof(IMailgunSettings.DomainName)] = @event.Settings.DomainName
    });
  }
  public void SetSendGridSettings(SenderSendGridSettingsChanged @event)
  {
    Update(@event);

    SetSettings(new Dictionary<string, string>
    {
      [nameof(ISendGridSettings.ApiKey)] = @event.Settings.ApiKey
    });
  }
  public void SetTwilioSettings(SenderTwilioSettingsChanged @event)
  {
    Update(@event);

    SetSettings(new Dictionary<string, string>
    {
      [nameof(ITwilioSettings.AccountSid)] = @event.Settings.AccountSid,
      [nameof(ITwilioSettings.AuthenticationToken)] = @event.Settings.AuthenticationToken
    });
  }

  public void Update(SenderUpdated @event)
  {
    base.Update(@event);

    if (@event.Email != null)
    {
      EmailAddress = @event.Email.Address;
    }
    if (@event.Phone != null)
    {
      PhoneNumber = @event.Phone.Number;
    }
    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }

  public Dictionary<string, string> GetSettings()
  {
    return (Settings == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Settings)) ?? [];
  }
  private void SetSettings(Dictionary<string, string> settings)
  {
    Settings = settings.Count < 1 ? null : JsonSerializer.Serialize(settings);
  }

  //public override string ToString()
  //{
  //  StringBuilder sender = new();
  //  switch (Type)
  //  {
  //    case SenderType.Email:
  //      if (Email == null)
  //      {
  //        throw new InvalidOperationException("The email has not been initialized.");
  //      }
  //      if (DisplayName == null)
  //      {
  //        sender.Append(Email.Address);
  //      }
  //      else
  //      {
  //        sender.Append(DisplayName.Value).Append(" <").Append(Email.Address).Append('>');
  //      }
  //      break;
  //    case SenderType.Phone:
  //      if (Phone == null)
  //      {
  //        throw new InvalidOperationException("The phone has not been initialized.");
  //      }
  //      sender.Append(Phone.FormatToE164());
  //      break;
  //    default:
  //      throw new SenderTypeNotSupportedException(Type);
  //  }
  //  sender.Append(" | ");
  //  sender.Append(base.ToString());
  //  return sender.ToString();
  //} // TODO(fpion): implement
}
