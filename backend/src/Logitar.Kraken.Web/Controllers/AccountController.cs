using FluentValidation;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Sessions.Commands;
using Logitar.Kraken.Core.Users.Commands;
using Logitar.Kraken.Web.Authentication;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Extensions;
using Logitar.Kraken.Web.Models.Account;
using Logitar.Kraken.Web.Validators.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly ILogger<AccountController> _logger;
  private readonly IMediator _mediator;
  private readonly IOpenAuthenticationService _openAuthenticationService;

  private new UserModel User => HttpContext.GetUser() ?? throw new InvalidOperationException("The User is required.");

  public AccountController(ILogger<AccountController> logger, IMediator mediator, IOpenAuthenticationService openAuthenticationService)
  {
    _logger = logger;
    _mediator = mediator;
    _openAuthenticationService = openAuthenticationService;
  }

  [HttpGet("/profile")]
  [Authorize(Policy = Policies.KrakenUser)]
  public ActionResult<UserModel> GetProfile() => Ok(User);

  [HttpPost("/auth/token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] GetTokenPayload input, CancellationToken cancellationToken)
  {
    new GetTokenValidator().ValidateAndThrow(input);

    try
    {
      SessionModel session;
      if (!string.IsNullOrWhiteSpace(input.RefreshToken))
      {
        RenewSessionPayload payload = new(input.RefreshToken.Trim(), HttpContext.GetSessionCustomAttributes());
        RenewSessionCommand command = new(payload);
        session = await _mediator.Send(command, cancellationToken);
      }
      else
      {
        SignInSessionPayload payload = new(input.Username ?? string.Empty, input.Password ?? string.Empty, isPersistent: true, HttpContext.GetSessionCustomAttributes());
        SignInSessionCommand command = new(payload);
        session = await _mediator.Send(command, cancellationToken);
      }
      TokenResponse response = await _openAuthenticationService.GetTokenResponseAsync(session, cancellationToken);
      return Ok(response);
    }
    catch (InvalidCredentialsException exception)
    {
      _logger.LogWarning(exception, "A get-token failure occurred.");

      Error error = new(code: "InvalidCredentials", message: "The specified credentials did not match.");
      return Problem(
        detail: error.Message,
        instance: Request.GetDisplayUrl(),
        statusCode: StatusCodes.Status400BadRequest,
        title: "Invalid Credentials",
        type: null,
        extensions: new Dictionary<string, object?> { ["error"] = error.Code });
    }
  }

  [HttpPatch("/profile")]
  [Authorize(Policy = Policies.KrakenUser)]
  public async Task<ActionResult<UserModel>> SaveProfileAsync([FromBody] UpdateProfilePayload profile, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = profile.ToUpdateUserPayload();
    UpdateUserCommand command = new(User.Id, payload);
    UserModel user = await _mediator.Send(command, cancellationToken) ?? throw new InvalidOperationException("The updated user should not be null.");
    return Ok(user);
  }

  [HttpPost("/sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload input, CancellationToken cancellationToken)
  {
    try
    {
      SignInSessionPayload payload = new(input.Username, input.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
      SignInSessionCommand command = new(payload);
      SessionModel session = await _mediator.Send(command, cancellationToken);
      HttpContext.SignIn(session);
      CurrentUser user = new(session);
      return Ok(user);
    }
    catch (InvalidCredentialsException exception)
    {
      _logger.LogWarning(exception, "A sign-in failure occurred.");

      Error error = new(code: "InvalidCredentials", message: "The specified credentials did not match.");
      return Problem(
        detail: error.Message,
        instance: Request.GetDisplayUrl(),
        statusCode: StatusCodes.Status400BadRequest,
        title: "Invalid Credentials",
        type: null,
        extensions: new Dictionary<string, object?> { ["error"] = error.Code });
    }
  }

  [HttpPost("/sign/out")]
  public async Task<ActionResult> SignOutAsync(bool everywhere, CancellationToken cancellationToken)
  {
    if (everywhere)
    {
      Guid? userId = HttpContext.GetUser()?.Id;
      if (userId.HasValue)
      {
        SignOutUserCommand command = new(userId.Value);
        await _mediator.Send(command, cancellationToken);
      }
    }
    else
    {
      Guid? sessionId = HttpContext.GetSessionId();
      if (sessionId.HasValue)
      {
        SignOutSessionCommand command = new(sessionId.Value);
        await _mediator.Send(command, cancellationToken);
      }
    }

    HttpContext.SignOut();
    return NoContent();
  }
}
