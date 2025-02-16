﻿using FluentValidation;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core.Users.Validators;

internal class AddressValidator : AbstractValidator<IAddress>
{
  public AddressValidator(IAddressHelper helper)
  {
    RuleFor(x => x.Street).NotEmpty().MaximumLength(Address.MaximumLength);

    RuleFor(x => x.Locality).NotEmpty().MaximumLength(Address.MaximumLength);

    When(x => helper.GetCountry(x.Country)?.PostalCode != null,
      () => RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(Address.MaximumLength)
        .Matches(address => helper.GetCountry(address.Country)!.PostalCode).WithErrorCode("PostalCodeValidator"))
      .Otherwise(() => When(x => x.PostalCode != null, () => RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(Address.MaximumLength)));

    When(x => helper.GetCountry(x.Country)?.Regions != null,
      () => RuleFor(x => x.Region).NotEmpty().MaximumLength(Address.MaximumLength)
        .Must((address, region) => helper.GetCountry(address.Country)!.Regions!.Contains(region)).WithErrorCode("RegionValidator")
          .WithMessage(address => $"'{{PropertyName}}' must be one of the following: {string.Join(", ", helper.GetCountry(address.Country)!.Regions!)}."))
      .Otherwise(() => When(x => x.Region != null, () => RuleFor(x => x.Region).NotEmpty().MaximumLength(Address.MaximumLength)));

    RuleFor(x => x.Country).NotEmpty().MaximumLength(Address.MaximumLength)
      .Must(helper.IsSupported).WithErrorCode("CountryValidator").WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", helper.SupportedCountries)}.");
  }
}
