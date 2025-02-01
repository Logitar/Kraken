using Logitar.EventSourcing;
using Logitar.Kraken.Core.Roles;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Events;

public record ApiKeyRoleAdded(RoleId RoleId) : DomainEvent, INotification;
