using Logitar.EventSourcing;
using Logitar.Kraken.Core.Fields;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentTypeFieldDefinitionChanged(FieldDefinition FieldDefinition) : DomainEvent, INotification;
