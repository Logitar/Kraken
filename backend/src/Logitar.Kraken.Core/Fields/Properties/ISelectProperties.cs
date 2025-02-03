namespace Logitar.Kraken.Core.Fields.Properties;

public interface ISelectProperties
{
  bool IsMultiple { get; }
  IReadOnlyCollection<SelectOption> Options { get; }
}
