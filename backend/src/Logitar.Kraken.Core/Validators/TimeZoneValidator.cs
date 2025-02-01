﻿using FluentValidation;
using FluentValidation.Validators;
using NodaTime;

namespace Logitar.Kraken.Core.Validators;

public class TimeZoneValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "TimeZoneValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' must correspond to a valid tz database entry ID.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    try
    {
      DateTimeZone? dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(value);
      return dateTimeZone != null;
    }
    catch (Exception)
    {
      return false;
    }
  }
}
