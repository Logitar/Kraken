using Logitar.EventSourcing;
using Logitar.Kraken.Core.Senders.Settings;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Events;

public record SenderMailgunSettingsChanged(MailgunSettings Settings) : DomainEvent, INotification;
