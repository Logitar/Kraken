using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Messages;

public interface IMessageManager
{
  Task<TemplateContent> CompileAsync(
    MessageId messageId,
    Template template,
    Dictionaries? dictionaries = null,
    Locale? locale = null,
    User? user = null,
    Variables? variables = null,
    CancellationToken cancellationToken = default);
  Task SendAsync(Message message, Sender sender, ActorId? actorId = null, CancellationToken cancellationToken = default);
}
