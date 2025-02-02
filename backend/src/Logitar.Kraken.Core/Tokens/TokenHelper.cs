using Logitar.Kraken.Contracts.Realms;

namespace Logitar.Kraken.Core.Tokens;

internal static class TokenHelper
{
  public static string ResolveAudience(string? audience, RealmModel? realm, string baseUrl)
  {
    if (!string.IsNullOrWhiteSpace(audience))
    {
      return FormatAudienceOrIssuer(audience.Trim(), realm, baseUrl);
    }
    else if (realm != null)
    {
      return realm.Url ?? realm.UniqueSlug;
    }

    return baseUrl;
  }
  public static string ResolveIssuer(string? issuer, RealmModel? realm, string baseUrl)
  {
    if (!string.IsNullOrWhiteSpace(issuer))
    {
      return FormatAudienceOrIssuer(issuer.Trim(), realm, baseUrl);
    }
    else if (realm != null)
    {
      return realm.Url ?? FormatAudienceOrIssuer("{BaseUrl}/realms/unique-slug:{UniqueSlug}", realm, baseUrl);
    }

    return baseUrl;
  }

  private static string FormatAudienceOrIssuer(string format, RealmModel? realm, string baseUrl)
  {
    if (realm != null)
    {
      format = format.Replace("{Id}", realm.Id.ToString())
        .Replace("{UniqueSlug}", realm.UniqueSlug)
        .Replace("{Url}", realm.Url);
    }

    return format.Replace("{BaseUrl}", baseUrl);
  }
}
