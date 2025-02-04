namespace Logitar.Kraken.Core.Senders;

public class CannotDeleteDefaultSenderException : Exception
{
  private const string ErrorMessage = "The default sender cannot be deleted, unless its the only sender in its Realm.";

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

  public CannotDeleteDefaultSenderException(Sender sender) : base(BuildMessage(sender))
  {
    RealmId = sender.RealmId?.ToGuid();
    SenderId = sender.EntityId;
  }

  private static string BuildMessage(Sender sender) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), sender.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(SenderId), sender.EntityId)
    .Build();
}
