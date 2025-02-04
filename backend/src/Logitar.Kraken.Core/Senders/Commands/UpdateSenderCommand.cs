using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders.Settings;
using Logitar.Kraken.Core.Senders.Validators;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Commands;

public record UpdateSenderCommand(Guid Id, UpdateSenderPayload Payload) : Activity, IRequest<SenderModel?>;

internal class UpdateSenderCommandHandler : IRequestHandler<UpdateSenderCommand, SenderModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public UpdateSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<SenderModel?> Handle(UpdateSenderCommand command, CancellationToken cancellationToken)
  {
    UpdateSenderPayload payload = command.Payload;
    new UpdateSenderValidator().ValidateAndThrow(payload);

    SenderId senderId = new(_applicationContext.RealmId, command.Id);
    Sender? sender = await _senderRepository.LoadAsync(senderId, cancellationToken);
    if (sender == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.EmailAddress))
    {
      sender.Email = new Email(payload.EmailAddress);
    }
    if (!string.IsNullOrWhiteSpace(payload.PhoneNumber))
    {
      sender.Phone = new Phone(payload.PhoneNumber);
    }
    if (payload.DisplayName != null)
    {
      sender.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      sender.Description = Description.TryCreate(payload.Description.Value);
    }

    SetSettings(payload, sender, actorId);

    sender.Update(actorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }

  private static void SetSettings(UpdateSenderPayload payload, Sender sender, ActorId? actorId)
  {
    if (payload.SendGrid != null)
    {
      SendGridSettings settings = new(payload.SendGrid);
      sender.SetSettings(settings, actorId);
    }
    if (payload.Mailgun != null)
    {
      MailgunSettings settings = new(payload.Mailgun);
      sender.SetSettings(settings, actorId);
    }
    if (payload.Twilio != null)
    {
      TwilioSettings settings = new(payload.Twilio);
      sender.SetSettings(settings, actorId);
    }
  }
}
