namespace Logitar.Kraken.Core.Users;

public interface IAddressHelper
{
  IReadOnlyCollection<string> SupportedCountries { get; }

  CountrySettings? GetCountry(string country);

  bool IsSupported(string country);
}
