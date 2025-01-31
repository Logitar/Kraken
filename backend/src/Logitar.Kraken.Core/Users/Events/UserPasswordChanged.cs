using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserPasswordChanged(Password Password) : DomainEvent, INotification;
