using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentLocaleUnpublished(LanguageId? LanguageId) : DomainEvent, INotification;
