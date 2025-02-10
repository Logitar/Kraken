using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Core.Messages;
using Logitar.Kraken.Core.Messages.Events;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class MessageEntity : AggregateEntity, ISegregatedEntity
{
  public int MessageId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string BodyType { get; private set; } = string.Empty;
  public string BodyText { get; private set; } = string.Empty;

  public int RecipientCount { get; private set; }
  public List<RecipientEntity> Recipients { get; private set; } = [];

  public SenderEntity? Sender { get; private set; }
  public int? SenderId { get; private set; }
  public bool SenderIsDefault { get; private set; }
  public string? SenderAddress { get; private set; }
  public string? SenderPhoneNumber { get; private set; }
  public string? SenderDisplayName { get; private set; }
  public SenderProvider SenderProvider { get; private set; }

  public TemplateEntity? Template { get; private set; }
  public int? TemplateId { get; private set; }
  public string TemplateUniqueKey { get; private set; } = string.Empty;
  public string? TemplateDisplayName { get; private set; }

  public bool IgnoreUserLocale { get; private set; }
  public string? Locale { get; private set; }

  public string? Variables { get; private set; }

  public bool IsDemo { get; private set; }

  public MessageStatus Status { get; private set; }
  public string? ResultData { get; private set; }

  public MessageEntity(RealmEntity? realm, SenderEntity sender, TemplateEntity template, Dictionary<string, UserEntity> users, MessageCreated @event) : base(@event)
  {
    if (realm != null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

    MessageId messageId = new(@event.StreamId);
    Id = messageId.EntityId;

    Subject = @event.Subject.Value;
    BodyType = @event.Body.Type;
    BodyText = @event.Body.Text;

    foreach (Recipient recipient in @event.Recipients)
    {
      UserEntity? user = null;
      if (recipient.UserId.HasValue && !users.TryGetValue(recipient.UserId.Value.Value, out user))
      {
        throw new InvalidOperationException($"The user entity 'StreamId={recipient.UserId.Value}' could not be found.");
      }

      Recipients.Add(new RecipientEntity(this, user, recipient));
    }
    RecipientCount = Recipients.Count;

    Sender = sender;
    SenderId = sender.SenderId;
    SenderIsDefault = @event.Sender.IsDefault;
    SenderAddress = @event.Sender.Email?.Address;
    SenderPhoneNumber = @event.Sender.Phone?.Number;
    SenderDisplayName = @event.Sender.DisplayName?.Value;
    SenderProvider = @event.Sender.Provider;

    Template = template;
    TemplateId = template.TemplateId;
    TemplateUniqueKey = @event.Template.UniqueKey.Value;
    TemplateDisplayName = @event.Template.DisplayName?.Value;

    IgnoreUserLocale = @event.IgnoreUserLocale;
    Locale = @event.Locale?.Code;

    Dictionary<string, string> variables = GetVariables();
    foreach (KeyValuePair<string, string> variable in @event.Variables)
    {
      variables[variable.Key] = variable.Value;
    }
    SetVariables(variables);

    IsDemo = @event.IsDemo;

    Status = MessageStatus.Unsent;
  }

  private MessageEntity() : base()
  {
  }

  public void Fail(MessageFailed @event)
  {
    Update(@event);

    Status = MessageStatus.Failed;
    SetResultData(@event.ResultData);
  }

  public void Succeed(MessageSucceeded @event)
  {
    Update(@event);

    Status = MessageStatus.Succeeded;
    SetResultData(@event.ResultData);
  }

  public TemplateContentModel GetBody() => new(BodyType, BodyText);

  public Dictionary<string, string> GetVariables()
  {
    return (Variables == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Variables)) ?? [];
  }
  private void SetVariables(Dictionary<string, string> variables)
  {
    Variables = variables.Count < 1 ? null : JsonSerializer.Serialize(variables);
  }

  public Dictionary<string, string> GetResultData()
  {
    return (ResultData == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(ResultData)) ?? [];
  }
  private void SetResultData(IReadOnlyDictionary<string, string> resultData)
  {
    ResultData = resultData.Count < 1 ? null : JsonSerializer.Serialize(resultData);
  }

  public override string ToString() => $"{Subject} | {base.ToString()}";
}
