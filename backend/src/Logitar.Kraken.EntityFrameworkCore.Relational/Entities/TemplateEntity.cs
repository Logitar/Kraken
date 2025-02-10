using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Templates.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class TemplateEntity : AggregateEntity, ISegregatedEntity
{
  public int TemplateId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public string UniqueKey { get; private set; } = string.Empty;
  public string UniqueKeyNormalized
  {
    get => Helper.Normalize(UniqueKey);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string ContentType { get; private set; } = string.Empty;
  public string ContentText { get; private set; } = string.Empty;

  public List<MessageEntity> Messages { get; private set; } = [];

  public TemplateEntity(RealmEntity? realm, TemplateCreated @event) : base(@event)
  {
    if (realm != null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

    TemplateId templateId = new(@event.StreamId);
    Id = templateId.EntityId;

    UniqueKey = @event.UniqueKey.Value;

    Subject = @event.Subject.Value;
    SetContent(@event.Content);
  }

  private TemplateEntity() : base()
  {
  }

  public void SetUniqueKey(TemplateUniqueKeyChanged @event)
  {
    Update(@event);

    UniqueKey = @event.UniqueKey.Value;
  }

  public void Update(TemplateUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Subject != null)
    {
      Subject = @event.Subject.Value;
    }
    if (@event.Content != null)
    {
      SetContent(@event.Content);
    }
  }

  public TemplateContentModel GetContent() => new(ContentType, ContentText);
  private void SetContent(ITemplateContent content)
  {
    ContentType = content.Type;
    ContentText = content.Text;
  }

  public override string ToString() => $"{DisplayName ?? UniqueKey} | {base.ToString()}";
}
