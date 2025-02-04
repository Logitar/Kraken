using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Senders.Settings;
using Logitar.Kraken.Core.Senders.Validators;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Commands;

public record CreateOrReplaceSenderResult(SenderModel? Sender = null, bool Created = false);

public record CreateOrReplaceSenderCommand(Guid? Id, CreateOrReplaceSenderPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceSenderResult>;

internal class CreateOrReplaceSenderCommandHandler : IRequestHandler<CreateOrReplaceSenderCommand, CreateOrReplaceSenderResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public CreateOrReplaceSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<CreateOrReplaceSenderResult> Handle(CreateOrReplaceSenderCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceSenderPayload payload = command.Payload;
    new CreateOrReplaceSenderValidator().ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    SenderId senderId = SenderId.NewId(realmId);
    Sender? sender = null;
    if (command.Id.HasValue)
    {
      senderId = new(realmId, command.Id.Value);
      sender = await _senderRepository.LoadAsync(senderId, cancellationToken);
    }

    ActorId? actorId = _applicationContext.ActorId;
    Email? email = string.IsNullOrWhiteSpace(payload.EmailAddress) ? null : new(payload.EmailAddress);
    Phone? phone = string.IsNullOrWhiteSpace(payload.PhoneNumber) ? null : new(payload.PhoneNumber);
    SenderSettings settings = GetSettings(payload);
    SenderType type = settings.Provider.GetSenderType();

    bool created = false;
    if (sender == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceSenderResult();
      }

      sender = new(email, phone, settings, actorId, senderId);
      created = true;

      // TODO(fpion): set default if there are no other senders of this type
    }

    Sender reference = (command.Version.HasValue
      ? await _senderRepository.LoadAsync(sender.Id, command.Version.Value, cancellationToken)
      : null) ?? sender;

    switch (type)
    {
      case SenderType.Email:
        if (email == null)
        {
          throw new InvalidOperationException("The sender email address is required.");
        }
        if (reference.Email != email)
        {
          sender.Email = email;
        }
        DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
        if (reference.DisplayName != displayName)
        {
          sender.DisplayName = displayName;
        }
        break;
      case SenderType.Phone:
        if (phone == null)
        {
          throw new InvalidOperationException("The sender phone number is required.");
        }
        if (reference.Phone != phone)
        {
          sender.Phone = phone;
        }
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }

    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      sender.Description = description;
    }

    sender.Update(actorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    SenderModel model = await _senderQuerier.ReadAsync(sender, cancellationToken);
    return new CreateOrReplaceSenderResult(model, created);
  }

  private static SenderSettings GetSettings(CreateOrReplaceSenderPayload payload)
  {
    List<SenderSettings> settings = new(capacity: 3);

    if (payload.Mailgun != null)
    {
      settings.Add(new MailgunSettings(payload.Mailgun));
    }
    if (payload.SendGrid != null)
    {
      settings.Add(new SendGridSettings(payload.SendGrid));
    }
    if (payload.Twilio != null)
    {
      settings.Add(new TwilioSettings(payload.Twilio));
    }

    if (settings.Count < 1)
    {
      throw new ArgumentException("The sender payload did not provide any settings.", nameof(payload));
    }
    else if (settings.Count > 1)
    {
      throw new ArgumentException($"The sender payload provided {settings.Count} settings.", nameof(payload));
    }
    return settings.Single();
  }
}
