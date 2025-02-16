using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Sessions.Validators;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Sessions.Commands;

/// <exception cref="IncorrectSessionSecretException"></exception>
/// <exception cref="InvalidRefreshTokenException"></exception>
/// <exception cref="SessionIsNotActiveException"></exception>
/// <exception cref="SessionIsNotPersistentException"></exception>
/// <exception cref="SessionNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public record RenewSessionCommand(RenewSessionPayload Payload) : Activity, IRequest<SessionModel>;

internal class RenewSessionCommandHandler : IRequestHandler<RenewSessionCommand, SessionModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public RenewSessionCommandHandler(
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<SessionModel> Handle(RenewSessionCommand command, CancellationToken cancellationToken)
  {
    RenewSessionPayload payload = command.Payload;
    new RenewSessionValidator().ValidateAndThrow(payload);

    ActorId? actorId = _applicationContext.ActorId;
    RealmId? realmId = _applicationContext.RealmId;

    RefreshToken refreshToken;
    try
    {
      refreshToken = RefreshToken.Decode(realmId, payload.RefreshToken);
    }
    catch (Exception innerException)
    {
      throw new InvalidRefreshTokenException(payload.RefreshToken, nameof(payload.RefreshToken), innerException);
    }

    Session session = await _sessionRepository.LoadAsync(refreshToken.Id, cancellationToken)
      ?? throw new SessionNotFoundException(refreshToken.Id, nameof(payload.RefreshToken));
    User user = await _userRepository.LoadAsync(session.UserId, cancellationToken)
      ?? throw new InvalidOperationException($"The user 'Id={session.UserId}' was not loaded.");
    if (user.RealmId != realmId)
    {
      throw new SessionNotFoundException(session.Id, nameof(payload.RefreshToken));
    }

    Password newSecret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    session.Renew(refreshToken.Secret, newSecret, actorId);
    foreach (CustomAttributeModel customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      session.SetCustomAttribute(key, customAttribute.Value);
    }
    session.Update(actorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    SessionModel result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    result.RefreshToken = new RefreshToken(session.Id, secretString).Encode();
    return result;
  }
}
