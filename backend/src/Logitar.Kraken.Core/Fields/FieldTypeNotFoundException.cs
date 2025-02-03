namespace Logitar.Kraken.Core.Fields;

public class FieldTypeNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified field type could not be found.";

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
      error.Data.Add(nameof(RealmId), RealmId);
      error.Data.Add(nameof(FieldTypeId), FieldTypeId);
      error.Data.Add(nameof(PropertyName), PropertyName);
      return error;
    }
  }

  public FieldTypeNotFoundException(FieldTypeId fieldTypeId, string propertyName) : base(BuildMessage(fieldTypeId, propertyName))
  {
    RealmId = fieldTypeId.RealmId?.ToGuid();
    FieldTypeId = fieldTypeId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(FieldTypeId fieldTypeId, string propertyName) => new ErrorMessageBuilder()
    .AddData(nameof(RealmId), fieldTypeId.RealmId?.ToGuid())
    .AddData(nameof(FieldTypeId), fieldTypeId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
