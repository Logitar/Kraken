namespace Logitar.Kraken.Core.Contents;

public class ContentAlreadyExistsException : ConflictException
{
  private const string ErrorMessage = "The specified content already exists.";

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
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ContentAlreadyExistsException(ContentId contentId, string propertyName) : base(BuildMessage(contentId, propertyName))
  {
    RealmId = contentId.RealmId?.ToGuid();
    ContentId = contentId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentId contentId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), contentId.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(ContentId), contentId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
