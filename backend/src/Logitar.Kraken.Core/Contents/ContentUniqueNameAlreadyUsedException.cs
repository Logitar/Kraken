using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.Core.Contents;

public class ContentUniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified content unique name is already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ContentTypeId
  {
    get => (Guid)Data[nameof(ContentTypeId)]!;
    private set => Data[nameof(ContentTypeId)] = value;
  }
  public Guid? LanguageId
  {
    get => (Guid?)Data[nameof(LanguageId)];
    private set => Data[nameof(LanguageId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
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
      error.Data[nameof(ContentTypeId)] = ContentTypeId;
      error.Data[nameof(LanguageId)] = LanguageId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ContentUniqueNameAlreadyUsedException(Content content, LanguageId? languageId, ContentLocale locale, ContentId conflictId)
    : base(BuildMessage(content, languageId, locale, conflictId))
  {
    RealmId = content.RealmId?.ToGuid();
    ContentTypeId = content.ContentTypeId.EntityId;
    LanguageId = languageId?.EntityId;
    ConflictId = conflictId.EntityId;
    EntityId = content.Id.EntityId;
    UniqueName = locale.UniqueName.Value;
    PropertyName = nameof(locale.UniqueName);
  }

  private static string BuildMessage(Content content, LanguageId? languageId, ContentLocale locale, ContentId conflictId)
  {
    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(RealmId), content.RealmId?.ToGuid(), "<null>")
      .AddData(nameof(ContentTypeId), content.ContentTypeId.EntityId)
      .AddData(nameof(LanguageId), languageId?.EntityId, "<null>")
      .AddData(nameof(ConflictId), conflictId.EntityId)
      .AddData(nameof(EntityId), content.Id.EntityId)
      .AddData(nameof(UniqueName), locale.UniqueName)
      .AddData(nameof(PropertyName), nameof(locale.UniqueName))
      .Build();
  }
}
