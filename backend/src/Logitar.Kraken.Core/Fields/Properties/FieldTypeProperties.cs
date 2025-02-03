using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Properties;

public abstract record FieldTypeProperties
{
  public abstract DataType DataType { get; }
}
