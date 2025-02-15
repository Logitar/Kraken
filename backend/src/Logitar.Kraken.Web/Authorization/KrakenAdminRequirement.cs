using Microsoft.AspNetCore.Authorization;

namespace Logitar.Kraken.Web.Authorization;

public record KrakenAdminRequirement : IAuthorizationRequirement;

public class KrakenAdminAuthorizationHandler : IAuthorizationHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public KrakenAdminAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Task HandleAsync(AuthorizationHandlerContext context)
  {
    return Task.CompletedTask; // TODO(fpion): implement
  }
}
