using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Contracts.Templates;

namespace Logitar.Kraken.Contracts.Messages;

public class MessageModel : AggregateModel
{
  public string Subject { get; set; } = string.Empty;
  public TemplateContentModel Body { get; set; } = new();

  public int RecipientCount { get; set; }
  public List<RecipientModel> Recipients { get; set; } = [];

  public SenderModel Sender { get; set; } = new();
  public TemplateModel Template { get; set; } = new();

  public bool IgnoreUserLocale { get; set; }
  public LocaleModel? Locale { get; set; }

  public List<Variable> Variables { get; set; } = [];

  public bool IsDemo { get; set; }

  public MessageStatus Status { get; set; }
  public List<ResultData> ResultData { get; set; } = [];

  public RealmModel? Realm { get; set; }
}
