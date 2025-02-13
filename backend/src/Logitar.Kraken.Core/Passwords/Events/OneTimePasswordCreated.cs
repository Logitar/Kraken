using Logitar.EventSourcing;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Passwords.Events;

public record OneTimePasswordCreated(Password Password, DateTime? ExpiresOn, int? MaximumAttempts, UserId? UserId) : DomainEvent, INotification;
