using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserUniqueNameChanged(UniqueName UniqueName) : DomainEvent, INotification;
