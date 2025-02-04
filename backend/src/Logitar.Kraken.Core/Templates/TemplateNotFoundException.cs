using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Templates;

public class TemplateNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified template could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string Identifier
  {
    get => (string)Data[nameof(Identifier)]!;
    private set => Data[nameof(Identifier)] = value;
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
      error.Data[nameof(Identifier)] = Identifier;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public TemplateNotFoundException(RealmId? realmId, string identifier, string propertyName) : base(BuildMessage(realmId, identifier, propertyName))
  {
    RealmId = realmId?.ToGuid();
    Identifier = identifier;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string identifier, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(Identifier), identifier)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
