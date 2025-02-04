using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Messages.Events;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Messages;

public class Message : AggregateRoot
{
  public new MessageId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private Subject? _subject = null;
  public Subject Subject => _subject ?? throw new InvalidOperationException("The message has not been initialized.");
  private TemplateContent? _body = null;
  public TemplateContent Body => _body ?? throw new InvalidOperationException("The message has not been initialized.");

  private readonly List<Recipient> _recipients = [];
  public IReadOnlyCollection<Recipient> Recipients => _recipients.AsReadOnly();
  private SenderSummary? _sender = null;
  public SenderSummary Sender => _sender ?? throw new InvalidOperationException("The message has not been initialized.");
  private TemplateSummary? _template = null;
  public TemplateSummary Template => _template ?? throw new InvalidOperationException("The message has not been initialized.");

  public bool IgnoreUserLocale { get; private set; }
  public Locale? Locale { get; private set; }

  private readonly Dictionary<string, string> _variables = [];
  public IReadOnlyDictionary<string, string> Variables => _variables.AsReadOnly();

  public bool IsDemo { get; private set; }

  public MessageStatus Status { get; private set; }
  private readonly Dictionary<string, string> _resultData = [];
  public IReadOnlyDictionary<string, string> ResultData => _resultData.AsReadOnly();

  public Message(
    Subject subject,
    TemplateContent body,
    IReadOnlyCollection<Recipient> recipients,
    Sender sender,
    Template template,
    bool ignoreUserLocale = false,
    Locale? locale = null,
    IReadOnlyDictionary<string, string>? variables = null,
    bool isDemo = false,
    ActorId? actorId = null,
    MessageId? messageId = null) : base(messageId?.StreamId)
  {
    List<UserId> notInRealm = new(capacity: recipients.Count);
    int to = 0;
    foreach (Recipient recipient in recipients)
    {
      if (recipient.Type == RecipientType.To)
      {
        to++;
      }

      if (recipient.User != null && recipient.User.RealmId != RealmId)
      {
        notInRealm.Add(recipient.User.Id);
      }
    }
    if (notInRealm.Count > 0)
    {
      throw new NotImplementedException(); // TODO(fpion): throw new UsersNotInTenantException(notInRealm, tenantId);
    }
    else if (to == 0)
    {
      throw new ToRecipientMissingException(this, nameof(recipients));
    }

    if (RealmId != sender.RealmId)
    {
      throw new NotImplementedException(); // TODO(fpion): throw new SenderNotInTenantException(sender, tenantId);
    }
    if (RealmId != template.RealmId)
    {
      throw new NotImplementedException(); // TODO(fpion): throw new TemplateNotInTenantException(template, tenantId);
    }

    variables ??= new Dictionary<string, string>();
    Raise(new MessageCreated(subject, body, recipients, new SenderSummary(sender), new TemplateSummary(template), ignoreUserLocale, locale, variables, isDemo), actorId);
  }
  protected virtual void Handle(MessageCreated @event)
  {
    _subject = @event.Subject;
    _body = @event.Body;

    _recipients.Clear();
    _recipients.AddRange(@event.Recipients);
    _sender = @event.Sender;
    _template = @event.Template;

    IgnoreUserLocale = @event.IgnoreUserLocale;
    Locale = @event.Locale;

    _variables.Clear();
    foreach (KeyValuePair<string, string> variable in @event.Variables)
    {
      _variables[variable.Key] = variable.Value;
    }

    IsDemo = @event.IsDemo;

    Status = MessageStatus.Unsent;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new MessageDeleted(), actorId);
    }
  }

  public void Fail(ActorId? actorId = null) => Fail(new Dictionary<string, string>(), actorId);
  public void Fail(IReadOnlyDictionary<string, string> resultData, ActorId? actorId = null)
  {
    if (Status == MessageStatus.Unsent)
    {
      Raise(new MessageFailed(resultData), actorId);
    }
  }
  protected virtual void Handle(MessageFailed @event)
  {
    Status = MessageStatus.Failed;

    _resultData.Clear();
    foreach (KeyValuePair<string, string> resultData in @event.ResultData)
    {
      _resultData[resultData.Key] = resultData.Value;
    }
  }

  public void Succeed(ActorId? actorId = null) => Succeed(new Dictionary<string, string>(), actorId);
  public void Succeed(IReadOnlyDictionary<string, string> resultData, ActorId? actorId = null)
  {
    if (Status == MessageStatus.Unsent)
    {
      Raise(new MessageSucceeded(resultData), actorId);
    }
  }
  protected virtual void Handle(MessageSucceeded @event)
  {
    Status = MessageStatus.Succeeded;

    _resultData.Clear();
    foreach (KeyValuePair<string, string> resultData in @event.ResultData)
    {
      _resultData[resultData.Key] = resultData.Value;
    }
  }

  public override string ToString() => $"{Subject.Value} | {base.ToString()}";
}
