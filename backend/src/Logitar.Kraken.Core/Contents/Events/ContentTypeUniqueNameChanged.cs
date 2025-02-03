using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentTypeUniqueNameChanged(Identifier UniqueName) : DomainEvent, INotification;
