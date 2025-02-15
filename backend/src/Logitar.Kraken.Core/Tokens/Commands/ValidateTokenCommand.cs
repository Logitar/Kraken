using FluentValidation;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Tokens;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Tokens.Validators;
using Logitar.Security.Claims;
using MediatR;

namespace Logitar.Kraken.Core.Tokens.Commands;

public record ValidateTokenCommand(ValidateTokenPayload Payload) : Activity, IRequest<ValidatedTokenModel>;

internal class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, ValidatedTokenModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISecretHelper _secretHelper;
  private readonly ITokenManager _tokenManager;

  public ValidateTokenCommandHandler(IApplicationContext applicationContext, ISecretHelper secretHelper, ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _secretHelper = secretHelper;
    _tokenManager = tokenManager;
  }

  public async Task<ValidatedTokenModel> Handle(ValidateTokenCommand command, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = command.Payload;
    new ValidateTokenValidator().ValidateAndThrow(payload);

    RealmModel? realm = _applicationContext.Realm;
    string baseUrl = _applicationContext.BaseUrl;

    string secret = _secretHelper.Resolve(payload.Secret);
    ValidateTokenOptions options = new()
    {
      ValidAudiences = [TokenHelper.ResolveAudience(payload.Audience, realm, baseUrl)],
      ValidIssuers = [TokenHelper.ResolveIssuer(payload.Issuer, realm, baseUrl)],
      Consume = payload.Consume
    };
    if (!string.IsNullOrWhiteSpace(payload.Type))
    {
      options.ValidTypes.Add(payload.Type.Trim());
    }
    ClaimsPrincipal principal = await _tokenManager.ValidateAsync(payload.Token, secret, options, cancellationToken);
    return CreateResult(principal);
  }

  private static ValidatedTokenModel CreateResult(ClaimsPrincipal principal)
  {
    ValidatedTokenModel result = new();

    string? emailAddress = null;
    bool? isEmailVerified = null;
    foreach (Claim claim in principal.Claims)
    {

      switch (claim.Type)
      {
        case Rfc7519ClaimNames.EmailAddress:
          emailAddress = claim.Value;
          break;
        case Rfc7519ClaimNames.IsEmailVerified:
          isEmailVerified = bool.Parse(claim.Value);
          break;
        case Rfc7519ClaimNames.Subject:
          result.Subject = claim.Value;
          break;
        default:
          result.Claims.Add(new(claim.Type, claim.Value, claim.ValueType));
          break;
      }
    }
    if (emailAddress != null)
    {
      result.Email = new EmailModel(emailAddress)
      {
        IsVerified = isEmailVerified ?? false
      };
    }
    else if (isEmailVerified.HasValue)
    {
      result.Claims.Add(new(Rfc7519ClaimNames.IsEmailVerified, isEmailVerified.Value.ToString()));
    }

    return result;
  }
}
