using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserAddressChanged(Address? Address) : DomainEvent, INotification;
