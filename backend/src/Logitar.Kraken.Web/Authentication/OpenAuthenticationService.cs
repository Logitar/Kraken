using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Realms;
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
  private const string RealmIdClaim = "realm_id";

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

    if (user.Realm != null)
    {
      payload.Claims.Add(new(RealmIdClaim, user.Realm.Id.ToString()));
    }
  }

  public async Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken)
  {
    ValidatedTokenModel identity = await ValidateAsync(accessToken, cancellationToken);
    UserModel user = ExtractUser(identity);
    if (user.Sessions.Count != 1)
    {
      throw new ArgumentException($"The access token did not contain a '{Rfc7519ClaimNames.SessionId}' claim.", nameof(accessToken));
    }
    return user.Sessions.Single();
  }
  public async Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken)
  {
    ValidatedTokenModel identity = await ValidateAsync(accessToken, cancellationToken);
    return ExtractUser(identity);
  }
  private async Task<ValidatedTokenModel> ValidateAsync(string accessToken, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = new(accessToken)
    {
      Type = _settings.AccessToken.Type
    };
    ValidateTokenCommand command = new(payload);
    return await _mediator.Send(command, cancellationToken);
  }
  private static UserModel ExtractUser(ValidatedTokenModel identity)
  {
    UserModel user = new();

    if (identity.Subject != null)
    {
      user.Id = Guid.Parse(identity.Subject);
    }

    if (identity.Email != null)
    {
      user.Email = identity.Email;
      user.IsConfirmed = user.Email.IsVerified;
    }

    Guid? sessionId = null;
    foreach (ClaimModel claim in identity.Claims)
    {
      switch (claim.Type)
      {
        case Rfc7519ClaimNames.AuthenticationTime:
          user.AuthenticatedOn = ClaimHelper.ExtractDateTime(new Claim(claim.Name, claim.Value, claim.Type));
          break;
        case Rfc7519ClaimNames.FirstName:
          user.FirstName = claim.Value;
          break;
        case Rfc7519ClaimNames.FullName:
          user.FullName = claim.Value;
          break;
        case Rfc7519ClaimNames.LastName:
          user.LastName = claim.Value;
          break;
        case Rfc7519ClaimNames.MiddleName:
          user.MiddleName = claim.Value;
          break;
        case Rfc7519ClaimNames.Picture:
          user.Picture = claim.Value;
          break;
        case Rfc7519ClaimNames.Roles:
          user.Roles.Add(new RoleModel
          {
            UniqueName = claim.Value
          });
          break;
        case Rfc7519ClaimNames.SessionId:
          sessionId = Guid.Parse(claim.Value);
          break;
        case Rfc7519ClaimNames.Username:
          user.UniqueName = claim.Value;
          break;
        case RealmIdClaim:
          user.Realm = new RealmModel
          {
            Id = Guid.Parse(claim.Value)
          };
          break;
      }
    }
    if (sessionId.HasValue)
    {
      ActorModel actor = new(user);
      SessionModel session = new()
      {
        Id = sessionId.Value,
        CreatedBy = actor,
        UpdatedBy = actor,
        IsActive = true,
        User = user
      };
      if (user.AuthenticatedOn.HasValue)
      {
        session.CreatedOn = user.AuthenticatedOn.Value;
        session.UpdatedOn = user.AuthenticatedOn.Value;
      }
      user.Sessions.Add(session);
    }

    return user;
  }
}
