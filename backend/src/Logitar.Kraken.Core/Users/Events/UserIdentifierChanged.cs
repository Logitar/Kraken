using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserIdentifierChanged(Identifier Key, CustomIdentifier Value) : DomainEvent, INotification;
