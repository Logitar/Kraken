namespace Logitar.Kraken.Core.ApiKeys;

/// <summary>
/// The exception raised when an API key secret check fails.
/// </summary>
public class IncorrectApiKeySecretException : /*InvalidCredentials*/Exception
{
  private const string ErrorMessage = "The specified secret did not match the API key.";

  public string ApiKeyId
  {
    get => (string)Data[nameof(ApiKeyId)]!;
    private set => Data[nameof(ApiKeyId)] = value;
  }
  public string AttemptedSecret
  {
    get => (string)Data[nameof(AttemptedSecret)]!;
    private set => Data[nameof(AttemptedSecret)] = value;
  }

  public IncorrectApiKeySecretException(ApiKey apiKey, string attemptedSecret)
    : base(BuildMessage(apiKey, attemptedSecret))
  {
    ApiKeyId = apiKey.Id.Value;
    AttemptedSecret = attemptedSecret;
  }

  private static string BuildMessage(ApiKey apiKey, string attemptedSecret) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ApiKeyId), apiKey.Id)
    .AddData(nameof(AttemptedSecret), attemptedSecret)
    .Build();
}
