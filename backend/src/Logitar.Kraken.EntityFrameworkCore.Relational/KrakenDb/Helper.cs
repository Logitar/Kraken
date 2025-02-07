namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Helper
{
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
