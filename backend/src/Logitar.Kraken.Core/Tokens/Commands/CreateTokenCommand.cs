using FluentValidation;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Tokens;
using Logitar.Kraken.Core.Tokens.Validators;
using Logitar.Kraken.Core.Users;
using Logitar.Security.Claims;
using MediatR;

namespace Logitar.Kraken.Core.Tokens.Commands;

/// <exception cref="ValidationException"></exception>
public record CreateTokenCommand(CreateTokenPayload Payload) : Activity, IRequest<CreatedTokenModel>;

internal class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, CreatedTokenModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISecretHelper _secretHelper;
  private readonly ITokenManager _tokenManager;

  public CreateTokenCommandHandler(IApplicationContext applicationContext, ISecretHelper secretHelper, ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _secretHelper = secretHelper;
    _tokenManager = tokenManager;
  }

  public async Task<CreatedTokenModel> Handle(CreateTokenCommand command, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = command.Payload;
    new CreateTokenValidator().ValidateAndThrow(payload);

    RealmModel? realm = _applicationContext.Realm;
    string baseUrl = _applicationContext.BaseUrl;

    ClaimsIdentity subject = CreateSubject(payload);
    string secret = _secretHelper.Resolve(payload.Secret);
    CreateTokenOptions options = new()
    {
      Audience = TokenHelper.ResolveAudience(payload.Audience, realm, baseUrl),
      Issuer = TokenHelper.ResolveIssuer(payload.Issuer, realm, baseUrl)
    };
    if (!string.IsNullOrWhiteSpace(payload.Algorithm))
    {
      options.SigningAlgorithm = payload.Algorithm.Trim();
    }
    if (!string.IsNullOrWhiteSpace(payload.Type))
    {
      options.Type = payload.Type.Trim();
    }
    if (payload.LifetimeSeconds.HasValue)
    {
      options.Expires = DateTime.UtcNow.AddSeconds(payload.LifetimeSeconds.Value);
    }
    string createdToken = await _tokenManager.CreateAsync(subject, secret, options, cancellationToken);
    return new CreatedTokenModel(createdToken);
  }

  private static ClaimsIdentity CreateSubject(CreateTokenPayload payload)
  {
    ClaimsIdentity subject = new();

    if (payload.IsConsumable)
    {
      subject.AddClaim(new(Rfc7519ClaimNames.TokenId, Guid.NewGuid().ToString()));
    }

    if (!string.IsNullOrWhiteSpace(payload.Subject))
    {
      subject.AddClaim(new(Rfc7519ClaimNames.Subject, payload.Subject.Trim()));
    }

    if (payload.Email != null)
    {
      Email email = new(payload.Email, payload.Email.IsVerified);
      subject.AddClaim(new(Rfc7519ClaimNames.EmailAddress, email.Address));
      subject.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, email.IsVerified.ToString()));
    }

    foreach (ClaimModel claim in payload.Claims)
    {
      subject.AddClaim(new(claim.Name, claim.Value, claim.Type));
    }

    return subject;
  }
}
