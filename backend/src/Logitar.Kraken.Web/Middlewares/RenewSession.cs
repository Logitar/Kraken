using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core.Sessions.Commands;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Extensions;
using MediatR;

namespace Logitar.Kraken.Web.Middlewares;

public class RenewSession
{
  private readonly RequestDelegate _next;

  public RenewSession(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, IMediator mediator)
  {
    if (!context.GetSessionId().HasValue)
    {
      if (context.Request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken) && refreshToken != null)
      {
        try
        {
          RenewSessionPayload payload = new(refreshToken, context.GetSessionCustomAttributes());
          RenewSessionCommand command = new(payload);
          SessionModel session = await mediator.Send(command);
          context.SignIn(session);
        }
        catch (Exception)
        {
          context.Response.Cookies.Delete(Cookies.RefreshToken);
        }
      }
    }

    await _next(context);
  }
}
