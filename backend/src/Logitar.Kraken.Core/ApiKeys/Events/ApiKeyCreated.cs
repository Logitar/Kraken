using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Events;

public record ApiKeyCreated(Password Secret, DisplayName Name) : DomainEvent, INotification;
