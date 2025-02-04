using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Events;

public record TemplateCreated(Identifier UniqueKey, Subject Subject, TemplateContent Content) : DomainEvent, INotification;
