using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields.Properties;

namespace Logitar.Kraken.Core.Fields;

public class UnexpectedFieldTypePropertiesException : BadRequestException
{
  private const string ErrorMessage = "The specified field type properties were not expected.";

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
  public DataType ExpectedDataType
  {
    get => (DataType)Data[nameof(ExpectedDataType)]!;
    private set => Data[nameof(ExpectedDataType)] = value;
  }
  public DataType ActualDataType
  {
    get => (DataType)Data[nameof(ActualDataType)]!;
    private set => Data[nameof(ActualDataType)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(FieldTypeId)] = FieldTypeId;
      error.Data[nameof(ExpectedDataType)] = ExpectedDataType;
      error.Data[nameof(ActualDataType)] = ActualDataType;
      return error;
    }
  }

  public UnexpectedFieldTypePropertiesException(FieldType fieldType, FieldTypeProperties settings) : base(BuildMessage(fieldType, settings))
  {
    RealmId = fieldType.RealmId?.ToGuid();
    FieldTypeId = fieldType.Id.EntityId;
    ExpectedDataType = fieldType.DataType;
    ActualDataType = settings.DataType;
  }

  private static string BuildMessage(FieldType fieldType, FieldTypeProperties settings) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(FieldTypeId), fieldType.RealmId?.ToGuid())
    .AddData(nameof(FieldTypeId), fieldType.Id.EntityId)
    .AddData(nameof(ExpectedDataType), fieldType.DataType)
    .AddData(nameof(ActualDataType), settings.DataType)
    .Build();
}
