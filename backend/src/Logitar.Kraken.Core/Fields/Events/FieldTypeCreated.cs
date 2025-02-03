using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Events;

public record FieldTypeCreated(UniqueName UniqueName, DataType DataType) : DomainEvent, INotification;
