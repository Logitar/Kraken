using Logitar.EventSourcing;
using Logitar.Kraken.Core.Senders.SendGrid;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Events;

public record SenderSendGridSettingsChanged(SendGridSettings Settings) : DomainEvent, INotification;
