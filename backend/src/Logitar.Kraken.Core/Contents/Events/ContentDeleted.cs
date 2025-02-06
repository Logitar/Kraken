using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentDeleted : DomainEvent, IDeleteEvent, INotification;
