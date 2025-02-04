using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Events;

public record TemplateDeleted : DomainEvent, IDeleteEvent, INotification;
