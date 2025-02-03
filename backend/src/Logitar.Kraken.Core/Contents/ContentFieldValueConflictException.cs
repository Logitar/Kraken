namespace Logitar.Kraken.Core.Contents;

public class ContentFieldValueConflictException : ConflictException
{
  private const string ErrorMessage = "The specified field values are already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ContentId
  {
    get => (Guid)Data[nameof(ContentId)]!;
    private set => Data[nameof(ContentId)] = value;
  }
  public IReadOnlyDictionary<Guid, Guid> ConflictIds
  {
    get => (IReadOnlyDictionary<Guid, Guid>)Data[nameof(ConflictIds)]!;
    private set => Data[nameof(ConflictIds)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(ContentId)] = ContentId;
      error.Data[nameof(ConflictIds)] = ConflictIds;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ContentFieldValueConflictException(ContentId contentId, IReadOnlyDictionary<Guid, ContentId> conflictIds, string propertyName)
    : base(BuildMessage(contentId, conflictIds, propertyName))
  {
    ContentId = contentId.EntityId;
    ConflictIds = conflictIds.ToDictionary(x => x.Key, x => x.Value.EntityId);
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentId contentId, IReadOnlyDictionary<Guid, ContentId> conflictIds, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(ContentId)).Append(": ").Append(contentId.EntityId).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);

    message.Append(nameof(ConflictIds)).Append(':').AppendLine();
    foreach (KeyValuePair<Guid, ContentId> conflictId in conflictIds)
    {
      message.Append(" - ").Append(conflictId.Key).Append(": ").Append(conflictId.Value.EntityId).AppendLine();
    }

    return message.ToString();
  }
}
