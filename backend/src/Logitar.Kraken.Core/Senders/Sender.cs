using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Senders.Events;
using Logitar.Kraken.Core.Senders.Settings;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Senders;

public class Sender : AggregateRoot
{
  private SenderUpdated _updated = new();

  public new SenderId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public bool IsDefault { get; private set; }

  private Email? _email = null;
  public Email? Email
  {
    get => _email;
    set
    {
      if (Type != SenderType.Email)
      {
        throw new InvalidOperationException($"The sender must be an {nameof(SenderType.Email)} sender in order to set its email address.");
      }
      else if (_email != value)
      {
        _email = value;
        _updated.Email = value;
      }
    }
  }
  private Phone? _phone = null;
  public Phone? Phone
  {
    get => _phone;
    set
    {
      if (Type != SenderType.Phone)
      {
        throw new InvalidOperationException($"The sender must be an {nameof(SenderType.Phone)} sender in order to set its phone number.");
      }
      else if (_phone != value)
      {
        _phone = value;
        _updated.Phone = value;
      }
    }
  }
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (Type != SenderType.Email)
      {
        throw new InvalidOperationException($"The sender must be an {nameof(SenderType.Email)} sender in order to set its display name.");
      }
      else if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  public SenderType Type => Provider.GetSenderType();
  public SenderProvider Provider { get; private set; }
  private SenderSettings? _settings = null;
  public SenderSettings Settings => _settings ?? throw new InvalidOperationException("The sender has not been initialized.");

  public Sender(Email? email, Phone? phone, SenderSettings settings, ActorId? actorId = null, SenderId? senderId = null) : base(senderId?.StreamId)
  {
    SenderProvider provider = settings.Provider;
    SenderType type = provider.GetSenderType();
    switch (type)
    {
      case SenderType.Email:
        ArgumentNullException.ThrowIfNull(email);
        break;
      case SenderType.Phone:
        ArgumentNullException.ThrowIfNull(phone);
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }
    Raise(new SenderCreated(email, phone, provider), actorId);

    switch (provider)
    {
      case SenderProvider.Mailgun:
        SetSettings((MailgunSettings)settings);
        break;
      case SenderProvider.SendGrid:
        SetSettings((SendGridSettings)settings);
        break;
      case SenderProvider.Twilio:
        SetSettings((TwilioSettings)settings);
        break;
      default:
        throw new SenderProviderNotSupportedException(provider);
    }
  }
  protected virtual void Handle(SenderCreated @event)
  {
    _email = @event.Email;
    _phone = @event.Phone;

    Provider = @event.Provider;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SenderDeleted(), actorId);
    }
  }

  public void SetDefault(bool isDefault = true, ActorId? actorId = null)
  {
    if (IsDefault != isDefault)
    {
      Raise(new SenderSetDefault(isDefault), actorId);
    }
  }
  protected virtual void Handle(SenderSetDefault @event)
  {
    IsDefault = @event.IsDefault;
  }

  public void SetSettings(MailgunSettings settings, ActorId? actorId = null)
  {
    if (Provider != SenderProvider.Mailgun)
    {
      throw new UnexpectedSenderSettingsException(this, settings);
    }
    else if (_settings != settings)
    {
      Raise(new SenderMailgunSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(SenderMailgunSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(SendGridSettings settings, ActorId? actorId = null)
  {
    if (Provider != SenderProvider.SendGrid)
    {
      throw new UnexpectedSenderSettingsException(this, settings);
    }
    else if (_settings != settings)
    {
      Raise(new SenderSendGridSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(SenderSendGridSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(TwilioSettings settings, ActorId? actorId = null)
  {
    if (Provider != SenderProvider.Twilio)
    {
      throw new UnexpectedSenderSettingsException(this, settings);
    }
    else if (_settings != settings)
    {
      Raise(new SenderTwilioSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(SenderTwilioSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(SenderUpdated @event)
  {
    if (@event.Email != null)
    {
      _email = @event.Email;
    }
    if (@event.Phone != null)
    {
      _phone = @event.Phone;
    }
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString()
  {
    StringBuilder sender = new();
    switch (Type)
    {
      case SenderType.Email:
        if (Email == null)
        {
          throw new InvalidOperationException("The email has not been initialized.");
        }
        if (DisplayName == null)
        {
          sender.Append(Email.Address);
        }
        else
        {
          sender.Append(DisplayName.Value).Append(" <").Append(Email.Address).Append('>');
        }
        break;
      case SenderType.Phone:
        if (Phone == null)
        {
          throw new InvalidOperationException("The phone has not been initialized.");
        }
        sender.Append(Phone.FormatToE164());
        break;
      default:
        throw new SenderTypeNotSupportedException(Type);
    }
    sender.Append(" | ");
    sender.Append(base.ToString());
    return sender.ToString();
  }
}
