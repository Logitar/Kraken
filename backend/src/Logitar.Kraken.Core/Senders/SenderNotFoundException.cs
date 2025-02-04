namespace Logitar.Kraken.Core.Senders;

public class SenderNotFoundException : Exception
{
  private const string ErrorMessage = "The specified sender could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid SenderId
  {
    get => (Guid)Data[nameof(SenderId)]!;
    private set => Data[nameof(SenderId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public SenderNotFoundException(SenderId senderId, string propertyName) : base(BuildMessage(senderId, propertyName))
  {
    RealmId = senderId.RealmId?.ToGuid();
    SenderId = senderId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(SenderId senderId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), senderId.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(SenderId), senderId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
