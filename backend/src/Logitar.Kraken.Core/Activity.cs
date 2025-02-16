namespace Logitar.Kraken.Core;

public abstract record Activity : IActivity
{
  public virtual IActivity Anonymize() => this;
}
