using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Passwords.Events;

public record OneTimePasswordCreated(Password Password, DateTime? ExpiresOn, int? MaximumAttempts) : DomainEvent, INotification;
