using Logitar.Kraken.Core.Realms;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.ApiKeys;

[Trait(Traits.Category, Categories.Unit)]
public class XApiKeyTests
{
  [Fact(DisplayName = "ctor: it should construct the correct X-API-Key.")]
  public void ctor_it_should_construct_the_correct_X_Api_Key()
  {
    ApiKeyId id = ApiKeyId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(XApiKey.SecretLength, out _);
    XApiKey xApiKey = new(id, secret);
    Assert.Equal(id, xApiKey.Id);
    Assert.Equal(secret, xApiKey.Secret);
  }

  [Fact(DisplayName = "ctor: it should throw ArgumentException when the secret length is not exact.")]
  public void ctor_it_should_throw_ArgumentException_when_the_secret_length_is_not_exact()
  {
    ApiKeyId id = ApiKeyId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(XApiKey.SecretLength * 2, out _);

    var exception = Assert.Throws<ArgumentException>(() => new XApiKey(id, secret));
    Assert.StartsWith("The secret must contain exactly 32 bytes.", exception.Message);
    Assert.Equal("secret", exception.ParamName);
  }

  [Theory(DisplayName = "Decode: it should decode the correct X-API-Key.")]
  [InlineData(null)]
  [InlineData("43cd1287-95ff-4d9a-b250-9190290e7d91")]
  public void Decode_it_should_decode_the_correct_X_Api_Key(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(Guid.Parse(realmIdValue));

    ApiKeyId id = ApiKeyId.NewId(realmId);
    string secret = RandomStringGenerator.GetBase64String(XApiKey.SecretLength, out _);
    string value = $"KK.{Convert.ToBase64String(id.EntityId.ToByteArray()).ToUriSafeBase64()}.{secret.ToUriSafeBase64()}";

    XApiKey xApiKey = XApiKey.Decode(realmId, value);
    Assert.Equal(id, xApiKey.Id);
    Assert.Equal(secret, xApiKey.Secret);
  }

  [Theory(DisplayName = "Decode: it should throw ArgumentException when the value is not valid.")]
  [InlineData("RT.abc.123")]
  [InlineData("abc.123")]
  public void Decode_it_should_throw_ArgumentException_when_the_value_is_not_valid(string value)
  {
    var exception = Assert.Throws<ArgumentException>(() => XApiKey.Decode(realmId: null, value));
    Assert.StartsWith($"The value '{value}' is not a valid X-API-Key.", exception.Message);
    Assert.Equal("value", exception.ParamName);
  }

  [Fact(DisplayName = "Encode: it should encode correctly the X-API-Key.")]
  public void Encode_it_should_encode_correctly_the_X_Api_Key()
  {
    ApiKeyId id = ApiKeyId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(XApiKey.SecretLength, out _);
    XApiKey xApiKey = new(id, secret);

    string encoded = $"KK.{id.Value}.{secret.ToUriSafeBase64()}";
    Assert.Equal(encoded, xApiKey.Encode());
  }

  [Fact(DisplayName = "Encode: it should encode correctly the X-API-Key from parts.")]
  public void Encode_it_should_encode_correctly_the_X_Api_Key_from_parts()
  {
    ApiKeyId id = ApiKeyId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(XApiKey.SecretLength, out _);

    string encoded = $"KK.{id.Value}.{secret.ToUriSafeBase64()}";
    Assert.Equal(encoded, new XApiKey(id, secret).Encode());
  }

  [Fact(DisplayName = "Encode: it should throw ArgumentException when the secret length is not exact.")]
  public void Encode_it_should_throw_ArgumentException_when_the_secret_length_is_not_exact()
  {
    ApiKeyId id = ApiKeyId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(XApiKey.SecretLength * 2, out _);

    var exception = Assert.Throws<ArgumentException>(() => new XApiKey(id, secret));
    Assert.StartsWith("The secret must contain exactly 32 bytes.", exception.Message);
    Assert.Equal("secret", exception.ParamName);
  }
}
