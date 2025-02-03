namespace Logitar.Kraken.Contracts.Fields;

public interface INumberProperties
{
  double? MinimumValue { get; }
  double? MaximumValue { get; }
  double? Step { get; }
}
