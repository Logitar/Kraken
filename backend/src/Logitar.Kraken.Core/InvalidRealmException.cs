using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core;

public class InvalidRealmException : InvalidOperationException
{
  private const string ErrorMessage = "The specified realm was not expected.";

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

  public InvalidRealmException(RealmId? expectedRealmId, RealmId? actualRealmId) : base(BuildMessage(expectedRealmId, actualRealmId))
  {
    ExpectedRealmId = expectedRealmId?.ToGuid();
    ActualRealmId = actualRealmId?.ToGuid();
  }

  private static string BuildMessage(RealmId? expectedRealmId, RealmId? actualRealmId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ExpectedRealmId), expectedRealmId?.ToGuid(), "<null>")
    .AddData(nameof(ActualRealmId), actualRealmId?.ToGuid(), "<null>")
    .Build();
}
