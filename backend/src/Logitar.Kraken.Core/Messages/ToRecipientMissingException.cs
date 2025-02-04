using Logitar.Kraken.Contracts.Messages;

namespace Logitar.Kraken.Core.Messages;

public class ToRecipientMissingException : BadRequestException
{
  private const string ErrorMessage = $"At least one {nameof(RecipientType.To)} recipient must be provided.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid MessageId
  {
    get => (Guid)Data[nameof(MessageId)]!;
    private set => Data[nameof(MessageId)] = value;
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
      error.Data[nameof(MessageId)] = MessageId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ToRecipientMissingException(Message message, string propertyName) : base(BuildMessage(message, propertyName))
  {
    RealmId = message.RealmId?.ToGuid();
    MessageId = message.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Message message, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), message.RealmId?.ToGuid())
    .AddData(nameof(MessageId), message.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
