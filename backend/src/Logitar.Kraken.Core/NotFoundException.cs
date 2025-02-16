namespace Logitar.Kraken.Core;

public abstract class NotFoundException : ErrorException
{
  public NotFoundException() : base()
  {
  }

  public NotFoundException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
