namespace Logitar.Kraken.Contracts.Fields;

public interface ISelectOption
{
  bool IsDisabled { get; }
  string Text { get; }
  string? Label { get; }
  string? Value { get; }
}
