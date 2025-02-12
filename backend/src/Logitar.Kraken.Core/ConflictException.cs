namespace Logitar.Kraken.Core;

public abstract class ConflictException : ErrorException
{
  public ConflictException() : base()
  {
  }

  public ConflictException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
