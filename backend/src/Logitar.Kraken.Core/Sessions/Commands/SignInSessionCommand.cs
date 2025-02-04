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

public record SignInSessionCommand(SignInSessionPayload Payload) : Activity, IRequest<SessionModel>
{
  public override IActivity Anonymize()
  {
    SignInSessionCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class SignInSessionCommandHandler : IRequestHandler<SignInSessionCommand, SessionModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;

  public SignInSessionCommandHandler(
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserManager userManager)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
  }

  public async Task<SessionModel> Handle(SignInSessionCommand command, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = command.Payload;
    new SignInSessionValidator().ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    SessionId sessionId = SessionId.NewId(realmId);
    Session? session;
    if (payload.Id.HasValue)
    {
      sessionId = new(realmId, payload.Id.Value);
      session = await _sessionRepository.LoadAsync(sessionId, cancellationToken);
      if (session != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value, nameof(payload.Id));
      }
    }

    User user = await _userManager.FindAsync(payload.UniqueName, nameof(payload.UniqueName), includeId: false, cancellationToken);

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }

    ActorId actorId = new(user.Id.Value);
    session = user.SignIn(payload.Password, secret, actorId, sessionId.EntityId);
    foreach (CustomAttributeModel customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      session.SetCustomAttribute(key, customAttribute.Value);
    }
    session.Update(actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    SessionModel result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    if (secretString != null)
    {
      result.RefreshToken = new RefreshToken(session.Id, secretString).Encode();
    }
    return result;
  }
}
