using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken;

public record Base64Password : Password
{
  public Base64Password(string password)
  {
  }

  public override string Encode()
  {
    throw new NotImplementedException();
  }

  public override bool IsMatch(string password)
  {
    throw new NotImplementedException();
  }
}
