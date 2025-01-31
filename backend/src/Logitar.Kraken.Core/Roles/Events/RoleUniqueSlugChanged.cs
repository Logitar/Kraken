using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Events;

public record RoleUniqueSlugChanged(UniqueName UniqueName) : DomainEvent, INotification;
