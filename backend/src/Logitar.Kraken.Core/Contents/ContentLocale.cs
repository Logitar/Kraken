using FluentValidation;

namespace Logitar.Kraken.Core.Contents;

public record ContentLocale
{
  public UniqueName UniqueName { get; }
  public DisplayName? DisplayName { get; }
  public Description? Description { get; }
  public IReadOnlyDictionary<Guid, string> FieldValues { get; }

  public ContentLocale(
    UniqueName uniqueName,
    DisplayName? displayName = null,
    Description? description = null,
    IReadOnlyDictionary<Guid, string>? fieldValues = null)
  {
    UniqueName = uniqueName;
    DisplayName = displayName;
    Description = description;

    Dictionary<Guid, string> cleanValues = [];
    if (fieldValues != null)
    {
      foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
      {
        if (!string.IsNullOrWhiteSpace(fieldValue.Value))
        {
          cleanValues[fieldValue.Key] = fieldValue.Value.Trim();
        }
      }
    }
    FieldValues = cleanValues.AsReadOnly();

    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<ContentLocale>
  {
    public Validator()
    {
      RuleForEach(x => x.FieldValues.Values).NotEmpty();
    }
  }
}
