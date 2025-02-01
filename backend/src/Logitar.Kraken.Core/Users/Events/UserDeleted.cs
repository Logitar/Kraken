using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserDeleted : DomainEvent, INotification;
