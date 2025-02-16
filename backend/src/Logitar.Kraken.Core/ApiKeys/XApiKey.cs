using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.ApiKeys;

internal record XApiKey
{
  public const int SecretLength = 256 / 8;

  private const string Prefix = "KK";
  private const char Separator = '.';

  public ApiKeyId Id { get; }
  public string Secret { get; }

  public XApiKey(ApiKeyId id, string secret)
  {
    if (Convert.FromBase64String(secret).Length != SecretLength)
    {
      throw new ArgumentException($"The secret must contain exactly {SecretLength} bytes.", nameof(secret));
    }

    Id = id;
    Secret = secret;
  }

  public static XApiKey Decode(RealmId? realmId, string value)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values.First() != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid X-API-Key.", nameof(value));
    }

    ApiKeyId id = new(new Guid(Convert.FromBase64String(values[1].FromUriSafeBase64())), realmId);
    string secret = values[2].FromUriSafeBase64();
    return new XApiKey(id, secret);
  }

  public string Encode() => string.Join(Separator, Prefix, Convert.ToBase64String(Id.EntityId.ToByteArray()).ToUriSafeBase64(), Secret.ToUriSafeBase64());
}
