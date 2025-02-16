namespace Logitar.Kraken.Core;

public abstract class InvalidCredentialsException : ErrorException
{
  public InvalidCredentialsException() : base()
  {
  }

  public InvalidCredentialsException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
