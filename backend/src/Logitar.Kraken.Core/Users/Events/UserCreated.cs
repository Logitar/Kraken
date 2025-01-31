using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserCreated(UniqueName UniqueName) : DomainEvent, INotification;
