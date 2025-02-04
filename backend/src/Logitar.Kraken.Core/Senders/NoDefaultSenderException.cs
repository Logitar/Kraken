using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Senders;

public class NoDefaultSenderException : NotFoundException
{
  private const string ErrorMessage = "The specified tenant has no default sender.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public SenderType Type
  {
    get => (SenderType)Data[nameof(Type)]!;
    private set => Data[nameof(Type)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(Type)] = Type;
      return error;
    }
  }

  public NoDefaultSenderException(RealmId? realmId, SenderType type) : base(BuildMessage(realmId, type))
  {
    RealmId = realmId?.ToGuid();
    Type = type;
  }

  private static string BuildMessage(RealmId? realmId, SenderType type) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(Type), type)
    .Build();
}
