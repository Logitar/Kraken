using Logitar.EventSourcing;
using Logitar.Kraken.Core.Senders.Settings;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Events;

public record SenderTwilioSettingsChanged(TwilioSettings Settings) : DomainEvent, INotification;
