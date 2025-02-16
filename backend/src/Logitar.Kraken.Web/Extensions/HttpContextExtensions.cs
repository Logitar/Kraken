using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Settings;
using Microsoft.Extensions.Primitives;

namespace Logitar.Kraken.Web.Extensions;

public static class HttpContextExtensions
{
  private const string ApiKeyKey = "ApiKey";
  private const string RealmKey = "Realm";
  private const string SessionKey = "Session";
  private const string SessionIdKey = "SessionId";
  private const string UserKey = "User";

  public static IReadOnlyCollection<CustomAttributeModel> GetSessionCustomAttributes(this HttpContext context)
  {
    List<CustomAttributeModel> customAttributes = new(capacity: 2)
    {
      new("AdditionalInformation", context.GetAdditionalInformation())
    };

    string? ipAddress = context.GetClientIpAddress();
    if (ipAddress != null)
    {
      customAttributes.Add(new("IpAddress", ipAddress));
    }

    return customAttributes.AsReadOnly();
  }
  public static string GetAdditionalInformation(this HttpContext context)
  {
    return JsonSerializer.Serialize(context.Request.Headers);
  }
  public static string? GetClientIpAddress(this HttpContext context)
  {
    string? ipAddress = null;

    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xForwardedFor))
    {
      ipAddress = xForwardedFor.Single()?.Split(':').First();
    }
    ipAddress ??= context.Connection.RemoteIpAddress?.ToString();

    return ipAddress;
  }

  public static ApiKeyModel? GetApiKey(this HttpContext context) => context.GetItem<ApiKeyModel>(ApiKeyKey);
  public static RealmModel? GetRealm(this HttpContext context) => context.GetItem<RealmModel>(RealmKey);
  public static SessionModel? GetSession(this HttpContext context) => context.GetItem<SessionModel>(SessionKey);
  public static UserModel? GetUser(this HttpContext context) => context.GetItem<UserModel>(UserKey);
  private static T? GetItem<T>(this HttpContext context, object key) => context.Items.TryGetValue(key, out object? value) ? (T?)value : default;

  public static void SetApiKey(this HttpContext context, ApiKeyModel? apiKey)
  {
    context.SetItem(ApiKeyKey, apiKey);
  }
  public static void SetRealm(this HttpContext context, RealmModel? realm)
  {
    context.SetItem(RealmKey, realm);
  }
  public static void SetSession(this HttpContext context, SessionModel? session)
  {
    context.SetItem(SessionKey, session);
  }
  public static void SetUser(this HttpContext context, UserModel? user)
  {
    context.SetItem(UserKey, user);
  }
  private static void SetItem(this HttpContext context, object key, object? value)
  {
    if (value == null)
    {
      context.Items.Remove(key);
    }
    else
    {
      context.Items[key] = value;
    }
  }

  public static Guid? GetSessionId(this HttpContext context)
  {
    byte[]? bytes = context.Session.Get(SessionIdKey);
    return bytes == null ? null : new Guid(bytes);
  }
  public static bool IsSignedIn(this HttpContext context) => context.GetSessionId().HasValue;
  public static void SignIn(this HttpContext context, SessionModel session)
  {
    context.Session.Set(SessionIdKey, session.Id.ToByteArray());

    if (session.RefreshToken != null)
    {
      CookiesSettings cookiesSettings = context.RequestServices.GetRequiredService<CookiesSettings>();
      CookieOptions options = new()
      {
        HttpOnly = cookiesSettings.RefreshToken.HttpOnly,
        MaxAge = cookiesSettings.RefreshToken.MaxAge,
        SameSite = cookiesSettings.RefreshToken.SameSite,
        Secure = cookiesSettings.RefreshToken.Secure
      };
      context.Response.Cookies.Append(Cookies.RefreshToken, session.RefreshToken, options);
    }

    context.SetSession(session);
    context.SetUser(session.User);
  }
  public static void SignOut(this HttpContext context)
  {
    context.Session.Clear();

    context.Response.Cookies.Delete(Cookies.RefreshToken);
  }
}
