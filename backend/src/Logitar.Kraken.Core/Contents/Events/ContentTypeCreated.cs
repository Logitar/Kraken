using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentTypeCreated(bool IsInvariant, Identifier UniqueName) : DomainEvent, INotification;
