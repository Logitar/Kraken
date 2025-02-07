using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Helper
{
  public static string Normalize(IEmail email) => Normalize(email.Address);
  public static string Normalize(Locale locale) => Normalize(locale.Code);
  public static string Normalize(UniqueName uniqueName) => Normalize(uniqueName.Value);
  public static string Normalize(Slug slug) => Normalize(slug.Value);
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
