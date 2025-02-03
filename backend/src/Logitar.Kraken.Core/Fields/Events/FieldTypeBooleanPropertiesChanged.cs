using Logitar.EventSourcing;
using Logitar.Kraken.Core.Fields.Properties;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Events;

public record FieldTypeBooleanPropertiesChanged(BooleanProperties Properties) : DomainEvent, INotification;
