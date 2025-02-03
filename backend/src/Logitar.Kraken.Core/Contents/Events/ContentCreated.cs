using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentCreated(ContentTypeId ContentTypeId, ContentLocale Invariant) : DomainEvent, INotification;
