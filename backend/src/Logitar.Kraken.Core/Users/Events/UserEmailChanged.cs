using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserEmailChanged(Email? Email) : DomainEvent, INotification;
