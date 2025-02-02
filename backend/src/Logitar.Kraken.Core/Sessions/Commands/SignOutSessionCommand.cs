using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Sessions.Commands;

public record SignOutSessionCommand(Guid Id) : Activity, IRequest<SessionModel?>;

internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, SessionModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutSessionCommandHandler(
    IApplicationContext applicationContext,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<SessionModel?> Handle(SignOutSessionCommand command, CancellationToken cancellationToken)
  {
    SessionId sessionId = new(_applicationContext.RealmId, command.Id);
    Session? session = await _sessionRepository.LoadAsync(sessionId, cancellationToken);
    if (session == null)
    {
      return null;
    }

    User user = await _userRepository.LoadAsync(session, cancellationToken);
    if (_applicationContext.RealmId != user.RealmId)
    {
      return null;
    }

    session.SignOut(_applicationContext.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}
