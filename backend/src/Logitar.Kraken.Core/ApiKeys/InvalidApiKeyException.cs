namespace Logitar.Kraken.Core.ApiKeys;

public class InvalidApiKeyException : /*InvalidCredentials*/Exception
{
  private const string ErrorMessage = "The specified value is not a valid API key.";

  public string ApiKey
  {
    get => (string)Data[nameof(ApiKey)]!;
    private set => Data[nameof(ApiKey)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public InvalidApiKeyException(string apiKey, string propertyName, Exception? innerException = null)
    : base(BuildMessage(apiKey, propertyName), innerException)
  {
    ApiKey = apiKey;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string apiKey, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ApiKey), apiKey)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
