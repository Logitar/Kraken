using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Events;

public record RoleDeleted : DomainEvent, INotification;
