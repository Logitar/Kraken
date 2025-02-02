using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Events;

public record RealmDeleted : DomainEvent, IDeleteEvent, INotification;
