using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentTypeFieldDefinitionRemoved(Guid FieldId) : DomainEvent, INotification;
