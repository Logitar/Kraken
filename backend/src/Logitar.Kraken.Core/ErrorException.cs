namespace Logitar.Kraken.Core;

public abstract class ErrorException : Exception
{
  public abstract Error Error { get; }

  public ErrorException() : base()
  {
  }

  public ErrorException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
