using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Kraken.Web.Authentication;

public class BearerAuthenticationOptions : AuthenticationSchemeOptions;

public class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
  private readonly IOpenAuthenticationService _openAuthenticationService;

  public BearerAuthenticationHandler(IOpenAuthenticationService openAuthenticationService, IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _openAuthenticationService = openAuthenticationService;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
    {
      foreach (string? value in authorization)
      {
        if (!string.IsNullOrWhiteSpace(value))
        {
          string[] values = value.Trim().Split();
          if (values.Length != 2)
          {
            return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
          }
          else if (values[0] == Schemes.Bearer)
          {
            try
            {
              string accessToken = values[1];
              UserModel user = await _openAuthenticationService.GetUserAsync(accessToken);
              Context.SetUser(user);

              ClaimsPrincipal principal;
              if (user.Sessions.Count == 1)
              {
                SessionModel session = user.Sessions.Single();
                Context.SetSession(session);

                principal = new(session.CreateClaimsIdentity(Scheme.Name));
              }
              else
              {
                principal = new(user.CreateClaimsIdentity(Scheme.Name));
              }
              AuthenticationTicket ticket = new(principal, Scheme.Name);

              return AuthenticateResult.Success(ticket);
            }
            catch (Exception exception)
            {
              return AuthenticateResult.Fail(exception);
            }
          }
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
