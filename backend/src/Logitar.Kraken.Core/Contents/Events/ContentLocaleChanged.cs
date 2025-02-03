using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentLocaleChanged(LanguageId? LanguageId, ContentLocale Locale) : DomainEvent, INotification;
