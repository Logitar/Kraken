namespace Logitar.Kraken.Core.ApiKeys;

public class ApiKeyIsExpiredException : /*InvalidCredentials*/Exception
{
  private const string ErrorMessage = "The specified API key is expired.";

  public string ApiKeyId
  {
    get => (string)Data[nameof(ApiKeyId)]!;
    private set => Data[nameof(ApiKeyId)] = value;
  }

  public ApiKeyIsExpiredException(ApiKey apiKey) : base(BuildMessage(apiKey))
  {
    ApiKeyId = apiKey.Id.Value;
  }

  private static string BuildMessage(ApiKey apiKey) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ApiKeyId), apiKey.Id)
    .Build();
}
