namespace Logitar.Kraken.Contracts.Users;
public interface IAddress
{
  string Street { get; }
  string Locality { get; }
  string? PostalCode { get; }
  string? Region { get; }
  string Country { get; }
}
