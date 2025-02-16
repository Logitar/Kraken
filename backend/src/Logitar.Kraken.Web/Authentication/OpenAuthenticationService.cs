using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Tokens;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Tokens.Commands;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Models.Account;
using Logitar.Kraken.Web.Settings;
using Logitar.Security.Claims;
using MediatR;

namespace Logitar.Kraken.Web.Authentication;

public class OpenAuthenticationService : IOpenAuthenticationService
{
  private readonly IMediator _mediator;
  private readonly OpenAuthenticationSettings _settings;

  public OpenAuthenticationService(IMediator mediator, OpenAuthenticationSettings settings)
  {
    _mediator = mediator;
    _settings = settings;
  }

  public async Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = new();
    PopulateClaims(payload, session);
    PopulateClaims(payload, session.User);

    return await GetTokenResponseAsync(payload, session.RefreshToken, cancellationToken);
  }
  public async Task<TokenResponse> GetTokenResponseAsync(UserModel user, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = new();
    PopulateClaims(payload, user);

    SessionModel? session = null;
    if (user.Sessions.Count == 1)
    {
      session = user.Sessions.Single();
      PopulateClaims(payload, session);
    }

    return await GetTokenResponseAsync(payload, session?.RefreshToken, cancellationToken);
  }
  private async Task<TokenResponse> GetTokenResponseAsync(CreateTokenPayload payload, string? refreshToken, CancellationToken cancellationToken)
  {
    AccessTokenSettings settings = _settings.AccessToken;

    payload.LifetimeSeconds = settings.Lifetime;
    payload.Type = settings.Type;
    CreateTokenCommand command = new(payload);
    CreatedTokenModel access = await _mediator.Send(command, cancellationToken);

    return new TokenResponse
    {
      AccessToken = access.Token,
      TokenType = Schemes.Bearer,
      ExpiresIn = settings.Lifetime,
      RefreshToken = refreshToken
    };
  }
  private static void PopulateClaims(CreateTokenPayload payload, SessionModel session)
  {
    payload.Claims.Add(new(Rfc7519ClaimNames.SessionId, session.Id.ToString()));
  }
  private static void PopulateClaims(CreateTokenPayload payload, UserModel user)
  {
    payload.Subject = user.Id.ToString();

    if (user.Email != null)
    {
      payload.Email = new EmailPayload(user.Email);
    }

    payload.Claims.Add(new(Rfc7519ClaimNames.Username, user.UniqueName));

    if (user.FullName != null)
    {
      payload.Claims.Add(new(Rfc7519ClaimNames.FullName, user.FullName));

      if (user.FirstName != null)
      {
        payload.Claims.Add(new(Rfc7519ClaimNames.FirstName, user.FirstName));
      }
      if (user.MiddleName != null)
      {
        payload.Claims.Add(new(Rfc7519ClaimNames.MiddleName, user.MiddleName));
      }
      if (user.LastName != null)
      {
        payload.Claims.Add(new(Rfc7519ClaimNames.LastName, user.LastName));
      }
    }

    if (user.Picture != null)
    {
      payload.Claims.Add(new(Rfc7519ClaimNames.Picture, user.Picture));
    }

    if (user.AuthenticatedOn.HasValue)
    {
      Claim claim = ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, user.AuthenticatedOn.Value);
      payload.Claims.Add(new(claim.Type, claim.Value, claim.ValueType));
    }

    foreach (RoleModel role in user.Roles)
    {
      payload.Claims.Add(new(Rfc7519ClaimNames.Roles, role.UniqueName));
    }
  }

  public Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
  public Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
