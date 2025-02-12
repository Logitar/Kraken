using FluentValidation;
using FluentValidation.Validators;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Validators;

internal class PhoneNumberValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "PhoneNumberValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' must be a valid phone number.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    try
    {
      Phone phone = new(value, countryCode: null, extension: null, isVerified: false);
      return true;
    }
    catch (Exception)
    {
      return false;
    }
  }
}
