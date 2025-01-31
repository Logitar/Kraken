using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken;

public record Base64Password : Password
{
  public Base64Password(string password)
  {
  }
}
