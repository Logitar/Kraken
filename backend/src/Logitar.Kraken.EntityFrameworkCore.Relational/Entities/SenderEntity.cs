using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Senders.Events;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class SenderEntity : AggregateEntity, ISegregatedEntity
{
  public int SenderId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

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
    if (realm != null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

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

  public MailgunSettingsModel? GetMailgunSettings()
  {
    if (Settings == null || Provider != SenderProvider.Mailgun)
    {
      return null;
    }
    return JsonSerializer.Deserialize<MailgunSettingsModel>(Settings);
  }
  public void SetMailgunSettings(SenderMailgunSettingsChanged @event)
  {
    Update(@event);

    MailgunSettingsModel settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }

  public SendGridSettingsModel? GetSendGridSettings()
  {
    if (Settings == null || Provider != SenderProvider.SendGrid)
    {
      return null;
    }
    return JsonSerializer.Deserialize<SendGridSettingsModel>(Settings);
  }
  public void SetSendGridSettings(SenderSendGridSettingsChanged @event)
  {
    Update(@event);

    SendGridSettingsModel settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }

  public TwilioSettingsModel? GetTwilioSettings()
  {
    if (Settings == null || Provider != SenderProvider.Twilio)
    {
      return null;
    }
    return JsonSerializer.Deserialize<TwilioSettingsModel>(Settings);
  }
  public void SetTwilioSettings(SenderTwilioSettingsChanged @event)
  {
    Update(@event);

    TwilioSettingsModel settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
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

  public override string ToString()
  {
    StringBuilder sender = new();
    switch (Type)
    {
      case SenderType.Email:
        if (DisplayName == null)
        {
          sender.Append(EmailAddress);

        }
        else
        {
          sender.Append(DisplayName).Append(" <").Append(EmailAddress).Append('>');
        }
        break;
      case SenderType.Phone:
        sender.Append(PhoneNumber);
        break;
        throw new SenderTypeNotSupportedException(Type);
    }
    sender.Append(" | ").Append(base.ToString());
    return sender.ToString();
  }
}
