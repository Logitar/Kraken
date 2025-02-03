namespace Logitar.Kraken.Contracts.Fields;

public interface IStringProperties
{
  int? MinimumLength { get; }
  int? MaximumLength { get; }
  string? Pattern { get; }
}
