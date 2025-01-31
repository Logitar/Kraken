using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Localization.Events;

public record LanguageCreated(bool IsDefault, Locale Locale) : DomainEvent, INotification;
