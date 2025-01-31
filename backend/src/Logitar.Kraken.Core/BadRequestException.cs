namespace Logitar.Kraken.Core;

public abstract class BadRequestException : ErrorException
{
  public BadRequestException() : base()
  {
  }

  public BadRequestException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
