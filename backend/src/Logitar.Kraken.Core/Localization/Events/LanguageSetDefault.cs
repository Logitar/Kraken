using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Localization.Events;

public record LanguageSetDefault(bool IsDefault) : DomainEvent, INotification;
