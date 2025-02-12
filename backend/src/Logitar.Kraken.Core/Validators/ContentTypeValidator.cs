using FluentValidation;
using FluentValidation.Validators;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Validators;

internal class ContentTypeValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _contentTypes = [MediaTypeNames.Text.Plain];

  public IReadOnlyCollection<string> ContentTypes => _contentTypes;
  public string Name { get; } = "ContentTypeValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be one of the following: {string.Join(", ", _contentTypes)}.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    return _contentTypes.Contains(value.ToLowerInvariant());
  }
}
