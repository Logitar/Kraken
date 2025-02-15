using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Web.Extensions;

public static class ClaimsExtensions // TODO(fpion): implement
{
  public static ClaimsIdentity CreateClaimsIdentity(this ApiKeyModel apiKey, string authenticationScheme)
  {
    throw new NotImplementedException();
  }
  public static ClaimsIdentity CreateClaimsIdentity(this SessionModel session, string authenticationScheme)
  {
    throw new NotImplementedException();
  }
  public static ClaimsIdentity CreateClaimsIdentity(this UserModel user, string authenticationScheme)
  {
    throw new NotImplementedException();
  }
}
