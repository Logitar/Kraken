using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Events;

public record ApiKeyDeleted : DomainEvent, INotification;
