namespace Logitar.Kraken.Core.Localization;

public class LanguageNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified language could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid LanguageId
  {
    get => (Guid)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
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
      error.Data.Add(nameof(RealmId), RealmId);
      error.Data.Add(nameof(LanguageId), LanguageId);
      error.Data.Add(nameof(PropertyName), PropertyName);
      return error;
    }
  }

  public LanguageNotFoundException(LanguageId languageId, string propertyName) : base(BuildMessage(languageId, propertyName))
  {
    RealmId = languageId.RealmId?.ToGuid();
    LanguageId = languageId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(LanguageId languageId, string propertyName) => new ErrorMessageBuilder()
    .AddData(nameof(RealmId), languageId.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(LanguageId), languageId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
