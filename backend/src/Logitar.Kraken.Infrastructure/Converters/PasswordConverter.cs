using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Infrastructure.Converters;

public class PasswordConverter : JsonConverter<Password>
{
  private readonly IPasswordManager _passwordManager;

  public PasswordConverter(IPasswordManager passwordManager)
  {
    _passwordManager = passwordManager;
  }

  public override Password? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : _passwordManager.Decode(value);
  }

  public override void Write(Utf8JsonWriter writer, Password password, JsonSerializerOptions options)
  {
    writer.WriteStringValue(password.Encode());
  }
}
