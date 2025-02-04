using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders;

public class UnexpectedSenderSettingsException : BadRequestException
{
  private const string ErrorMessage = "The specified sender settings were not expected.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid FieldTypeId
  {
    get => (Guid)Data[nameof(FieldTypeId)]!;
    private set => Data[nameof(FieldTypeId)] = value;
  }
  public SenderProvider ExpectedSenderProvider
  {
    get => (SenderProvider)Data[nameof(ExpectedSenderProvider)]!;
    private set => Data[nameof(ExpectedSenderProvider)] = value;
  }
  public SenderProvider ActualSenderProvider
  {
    get => (SenderProvider)Data[nameof(ActualSenderProvider)]!;
    private set => Data[nameof(ActualSenderProvider)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(FieldTypeId)] = FieldTypeId;
      error.Data[nameof(ExpectedSenderProvider)] = ExpectedSenderProvider;
      error.Data[nameof(ActualSenderProvider)] = ActualSenderProvider;
      return error;
    }
  }

  public UnexpectedSenderSettingsException(Sender sender, SenderSettings settings) : base(BuildMessage(sender, settings))
  {
    RealmId = sender.RealmId?.ToGuid();
    FieldTypeId = sender.EntityId;
    ExpectedSenderProvider = sender.Provider;
    ActualSenderProvider = settings.Provider;
  }

  private static string BuildMessage(Sender sender, SenderSettings settings) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(FieldTypeId), sender.RealmId?.ToGuid())
    .AddData(nameof(FieldTypeId), sender.EntityId)
    .AddData(nameof(ExpectedSenderProvider), sender.Provider)
    .AddData(nameof(ActualSenderProvider), settings.Provider)
    .Build();
}
