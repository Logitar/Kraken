using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Messages;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Infrastructure.Messages;

internal class MessageManager : IMessageManager // ISSUE #94: https://github.com/Logitar/Kraken/issues/94
{
  public Task<TemplateContent> CompileAsync(MessageId messageId, Template template, Dictionaries? dictionaries, Locale? locale, User? user, Variables? variables, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task SendAsync(Message message, Sender sender, ActorId? actorId, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
