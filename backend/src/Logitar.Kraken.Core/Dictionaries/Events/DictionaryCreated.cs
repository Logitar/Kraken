using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Events;

public record DictionaryCreated(LanguageId LanguageId) : DomainEvent, INotification;
