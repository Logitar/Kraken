using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentTypeDeleted : DomainEvent, IDeleteEvent, INotification;
