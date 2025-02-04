using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders;

public class CannotDeleteDefaultSenderException : BadRequestException
{
  private const string ErrorMessage = "The default sender cannot be deleted, unless its the only sender of its type in its Realm.";

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
  public SenderType SenderType
  {
    get => (SenderType)Data[nameof(SenderType)]!;
    private set => Data[nameof(SenderType)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(SenderId)] = SenderId;
      error.Data[nameof(SenderType)] = SenderType;
      return error;
    }
  }

  public CannotDeleteDefaultSenderException(Sender sender) : base(BuildMessage(sender))
  {
    RealmId = sender.RealmId?.ToGuid();
    SenderId = sender.EntityId;
    SenderType = sender.Type;
  }

  private static string BuildMessage(Sender sender) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), sender.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(SenderId), sender.EntityId)
    .AddData(nameof(SenderType), sender.Type)
    .Build();
}
