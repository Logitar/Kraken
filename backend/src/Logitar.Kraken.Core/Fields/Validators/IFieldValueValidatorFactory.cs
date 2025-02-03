namespace Logitar.Kraken.Core.Fields.Validators;

public interface IFieldValueValidatorFactory
{
  IFieldValueValidator Create(FieldType fieldType);
}
