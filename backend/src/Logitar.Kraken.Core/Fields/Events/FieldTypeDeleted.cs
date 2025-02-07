using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Events;

public record FieldTypeDeleted : DomainEvent, IDeleteEvent, INotification;
