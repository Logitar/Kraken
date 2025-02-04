using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Messages.Events;

public record MessageSucceeded(IReadOnlyDictionary<string, string> ResultData) : DomainEvent, INotification;
