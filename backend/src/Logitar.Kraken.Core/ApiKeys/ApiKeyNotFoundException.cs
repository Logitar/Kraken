namespace Logitar.Kraken.Core.ApiKeys;

public class ApiKeyNotFoundException : /*InvalidCredentials*/Exception
{
  private const string ErrorMessage = "The specified API key could not be found.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)]!;
    private set => Data[nameof(TenantId)] = value;
  }
  public Guid ApiKeyId
  {
    get => (Guid)Data[nameof(ApiKeyId)]!;
    private set => Data[nameof(ApiKeyId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ApiKeyNotFoundException(ApiKeyId id, string propertyName) : base(BuildMessage(id, propertyName))
  {
    //TenantId = id.TenantId?.ToGuid();
    //ApiKeyId = id.EntityId.ToGuid();
    //PropertyName = propertyName;
  }

  private static string BuildMessage(ApiKeyId id, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    //.AddData(nameof(TenantId), id.TenantId?.ToGuid(), "<null>")
    //.AddData(nameof(ApiKeyId), id.EntityId.ToGuid())
    //.AddData(nameof(PropertyName), propertyName)
    .Build();
}
