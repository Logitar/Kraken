using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Templates;
using MediatR;

namespace Logitar.Kraken.Core.Messages.Events;

public record MessageCreated(
  Subject Subject,
  TemplateContent Body,
  IReadOnlyCollection<Recipient> Recipients,
  SenderSummary Sender,
  TemplateSummary Template,
  bool IgnoreUserLocale,
  Locale? Locale,
  IReadOnlyDictionary<string, string> Variables,
  bool IsDemo) : DomainEvent, INotification;
