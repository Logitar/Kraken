namespace Logitar.Kraken.Core.Fields;

public interface IFieldTypeManager
{
  Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken = default);
}
