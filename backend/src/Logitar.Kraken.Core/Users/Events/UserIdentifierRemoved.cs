using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserIdentifierRemoved(Identifier Key) : DomainEvent, INotification;
