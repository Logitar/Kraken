using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Kraken.Web.Authorization;

public record KrakenUserRequirement : IAuthorizationRequirement;

public class KrakenUserAuthorizationHandler : AuthorizationHandler<KrakenUserRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public KrakenUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, KrakenUserRequirement requirement)
  {
    HttpContext? httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null)
    {
      UserModel? user = httpContext.GetUser();
      if (user == null)
      {
        context.Fail(new AuthorizationFailureReason(this, "The User should not be null."));
      }
      else if (user.Realm != null)
      {
        context.Fail(new AuthorizationFailureReason(this, "The User should not reside into a Realm."));
      }
      else
      {
        context.Succeed(requirement);
      }
    }

    return Task.CompletedTask;
  }
}
