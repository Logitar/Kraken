using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core;

public class RealmMismatchException : ErrorException
{
  private const string ErrorMessage = "The actual realm did not match the expected realm.";

  public Guid? ExpectedRealmId
  {
    get => (Guid?)Data[nameof(ExpectedRealmId)];
    private set => Data[nameof(ExpectedRealmId)] = value;
  }
  public Guid? ActualRealmId
  {
    get => (Guid?)Data[nameof(ActualRealmId)];
    private set => Data[nameof(ActualRealmId)] = value;
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
      error.Data[nameof(ExpectedRealmId)] = ExpectedRealmId;
      error.Data[nameof(ActualRealmId)] = ActualRealmId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public RealmMismatchException(RealmId? expectedRealmId, RealmId? actualRealmId, string propertyName) : base(BuildMessage(expectedRealmId, actualRealmId, propertyName))
  {
    ExpectedRealmId = expectedRealmId?.ToGuid();
    ActualRealmId = actualRealmId?.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? expectedRealmId, RealmId? actualRealmId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ExpectedRealmId), expectedRealmId)
    .AddData(nameof(ActualRealmId), actualRealmId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
