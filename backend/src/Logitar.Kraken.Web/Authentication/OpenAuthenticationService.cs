using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Tokens;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Tokens.Commands;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Models.Account;
using Logitar.Kraken.Web.Settings;
using MediatR;

namespace Logitar.Kraken.Web.Authentication;

public class OpenAuthenticationService : IOpenAuthenticationService // TODO(fpion): implement
{
  private readonly IMediator _mediator;
  private readonly OpenAuthenticationSettings _settings;

  public OpenAuthenticationService(IMediator mediator, OpenAuthenticationSettings settings)
  {
    _mediator = mediator;
    _settings = settings;
  }

  public Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public Task<TokenResponse> GetTokenResponseAsync(UserModel user, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
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

  public Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
