using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Localization.Events;

public record LanguageLocaleChanged(Locale Locale) : DomainEvent, INotification;
