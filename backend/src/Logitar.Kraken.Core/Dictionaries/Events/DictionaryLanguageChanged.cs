using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Events;

public record DictionaryLanguageChanged(LanguageId LanguageId) : DomainEvent, INotification;
