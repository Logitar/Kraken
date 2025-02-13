using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Events;

public record RoleUniqueNameChanged(UniqueName UniqueName) : DomainEvent, INotification;
