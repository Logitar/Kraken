using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Events;

public record RoleCreated(UniqueName UniqueName) : DomainEvent, INotification;
