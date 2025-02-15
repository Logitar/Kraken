using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Web.Extensions;

public static class HttpContextExtensions // TODO(fpion): implement
{
  public static ApiKeyModel? GetApiKey(this HttpContext context) => throw new NotImplementedException();
  public static RealmModel? GetRealm(this HttpContext context) => throw new NotImplementedException();
  public static SessionModel? GetSession(this HttpContext context) => throw new NotImplementedException();
  public static UserModel? GetUser(this HttpContext context) => throw new NotImplementedException();

  public static void SetApiKey(this HttpContext context, ApiKeyModel apiKey) => throw new NotImplementedException();
  public static void SetRealm(this HttpContext context, RealmModel realm) => throw new NotImplementedException();
  public static void SetSession(this HttpContext context, SessionModel session) => throw new NotImplementedException();
  public static void SetUser(this HttpContext context, UserModel user) => throw new NotImplementedException();

  public static Guid? GetSessionId(this HttpContext context)
  {
    throw new NotImplementedException();
  }
  public static void SignOut(this HttpContext context)
  {
    throw new NotImplementedException();
  }
}
