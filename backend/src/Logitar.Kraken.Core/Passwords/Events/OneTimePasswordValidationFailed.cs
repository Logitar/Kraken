using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Passwords.Events;

public record OneTimePasswordValidationFailed : DomainEvent, INotification;
