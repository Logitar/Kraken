using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Events;

public record TemplateUniqueKeyChanged(Identifier UniqueKey) : DomainEvent, INotification;
