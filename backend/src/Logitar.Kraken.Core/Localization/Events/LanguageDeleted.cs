using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Localization.Events;

public record LanguageDeleted : DomainEvent, IDeleteEvent, INotification;
