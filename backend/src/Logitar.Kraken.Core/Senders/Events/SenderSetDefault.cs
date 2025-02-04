using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Events;

public record SenderSetDefault(bool IsDefault) : DomainEvent, INotification;
