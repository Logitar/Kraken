using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Events;

public record FieldTypeUniqueNameChanged(UniqueName UniqueName) : DomainEvent, INotification;
