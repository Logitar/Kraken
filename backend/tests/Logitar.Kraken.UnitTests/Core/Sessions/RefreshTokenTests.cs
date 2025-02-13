using Logitar.Kraken.Core.Realms;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.Sessions;

[Trait(Traits.Category, Categories.Unit)]
public class RefreshTokenTests
{
  [Fact(DisplayName = "ctor: it should construct the correct refresh token.")]
  public void ctor_it_should_construct_the_correct_refresh_token()
  {
    SessionId id = SessionId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    RefreshToken refreshToken = new(id, secret);
    Assert.Equal(id, refreshToken.Id);
    Assert.Equal(secret, refreshToken.Secret);
  }

  [Fact(DisplayName = "ctor: it should throw ArgumentException when the secret length is not exact.")]
  public void ctor_it_should_throw_ArgumentException_when_the_secret_length_is_not_exact()
  {
    SessionId id = SessionId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength * 2, out _);

    var exception = Assert.Throws<ArgumentException>(() => new RefreshToken(id, secret));
    Assert.StartsWith("The secret must contain exactly 32 bytes.", exception.Message);
    Assert.Equal("secret", exception.ParamName);
  }

  [Theory(DisplayName = "Decode: it should decode the correct refresh token.")]
  [InlineData(null)]
  [InlineData("41ea3870-2a65-4513-8638-68afa7b943aa")]
  public void Decode_it_should_decode_the_correct_refresh_token(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(Guid.Parse(realmIdValue));

    SessionId id = SessionId.NewId(realmId);
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    string value = $"RT.{Convert.ToBase64String(id.EntityId.ToByteArray()).ToUriSafeBase64()}.{secret.ToUriSafeBase64()}";

    RefreshToken refreshToken = RefreshToken.Decode(realmId, value);
    Assert.Equal(id, refreshToken.Id);
    Assert.Equal(secret, refreshToken.Secret);
  }

  [Theory(DisplayName = "Decode: it should throw ArgumentException when the value is not valid.")]
  [InlineData("KK.abc.123")]
  [InlineData("abc.123")]
  public void Decode_it_should_throw_ArgumentException_when_the_value_is_not_valid(string value)
  {
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Decode(realmId: null, value));
    Assert.StartsWith($"The value '{value}' is not a valid refresh token.", exception.Message);
    Assert.Equal("value", exception.ParamName);
  }

  [Fact(DisplayName = "Encode: it should encode correctly the refresh token.")]
  public void Encode_it_should_encode_correctly_the_refresh_token()
  {
    SessionId id = SessionId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    RefreshToken refreshToken = new(id, secret);

    string encoded = $"RT.{id.Value}.{secret.ToUriSafeBase64()}";
    Assert.Equal(encoded, refreshToken.Encode());
  }

  [Fact(DisplayName = "Encode: it should encode correctly the refresh token from parts.")]
  public void Encode_it_should_encode_correctly_the_refresh_token_from_parts()
  {
    SessionId id = SessionId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);

    string encoded = $"RT.{id.Value}.{secret.ToUriSafeBase64()}";
    Assert.Equal(encoded, new RefreshToken(id, secret).Encode());
  }

  [Fact(DisplayName = "Encode: it should throw ArgumentException when the secret length is not exact.")]
  public void Encode_it_should_throw_ArgumentException_when_the_secret_length_is_not_exact()
  {
    SessionId id = SessionId.NewId(realmId: null);
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength * 2, out _);

    var exception = Assert.Throws<ArgumentException>(() => new RefreshToken(id, secret));
    Assert.StartsWith("The secret must contain exactly 32 bytes.", exception.Message);
    Assert.Equal("secret", exception.ParamName);
  }
}
