using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserPhoneChanged(Phone? Phone) : DomainEvent, INotification;
