using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Kraken.Web.Authorization;

public record KrakenAdminRequirement : IAuthorizationRequirement;

public class KrakenAdminAuthorizationHandler : AuthorizationHandler<KrakenAdminRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public KrakenAdminAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, KrakenAdminRequirement requirement)
  {
    HttpContext? httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null)
    {
      ApiKeyModel? apiKey = httpContext.GetApiKey();
      if (apiKey != null)
      {
        if (apiKey.Realm == null)
        {
          context.Succeed(requirement);
        }
        else
        {
          context.Fail(new AuthorizationFailureReason(this, "The API key should not reside into a Realm."));
        }
      }

      UserModel? user = httpContext.GetUser();
      if (user != null)
      {
        if (user.Realm == null)
        {
          context.Succeed(requirement);
        }
        else
        {
          context.Fail(new AuthorizationFailureReason(this, "The User should not reside into a Realm."));
        }
      }
    }

    return Task.CompletedTask;
  }
}
