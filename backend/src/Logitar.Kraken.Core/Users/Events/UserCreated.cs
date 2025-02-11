using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserCreated(UniqueName UniqueName, Password? Password) : DomainEvent, INotification;
