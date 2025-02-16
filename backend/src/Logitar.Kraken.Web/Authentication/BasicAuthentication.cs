using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Users.Commands;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Kraken.Web.Authentication;

public class BasicAuthenticationOptions : AuthenticationSchemeOptions;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  private readonly IMediator _mediator;

  public BasicAuthenticationHandler(IMediator mediator, IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _mediator = mediator;
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
          else if (values[0] == Schemes.Basic)
          {
            byte[] bytes = Convert.FromBase64String(values[1]);
            string credentials = Encoding.UTF8.GetString(bytes);
            int index = credentials.IndexOf(':');
            if (index <= 0)
            {
              return AuthenticateResult.Fail($"The Basic credentials are not valid: '{credentials}'.");
            }

            try
            {
              string uniqueName = credentials[..index];
              string password = credentials[(index + 1)..];
              AuthenticateUserPayload payload = new(uniqueName, password);
              AuthenticateUserCommand command = new(payload);
              UserModel user = await _mediator.Send(command);

              Context.SetUser(user);

              ClaimsPrincipal principal = new(user.CreateClaimsIdentity(Scheme.Name));
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
