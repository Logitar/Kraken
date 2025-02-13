using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Sessions;
using MediatR;

namespace Logitar.Kraken.Core.Users.Commands;

public record SignOutUserCommand(Guid Id) : Activity, IRequest<UserModel?>;

internal class SignOutUserCommandHandler : IRequestHandler<SignOutUserCommand, UserModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SignOutUserCommandHandler(
    IApplicationContext applicationContext,
    ISessionRepository sessionRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _sessionRepository = sessionRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(SignOutUserCommand command, CancellationToken cancellationToken)
  {
    UserId userId = new(command.Id, _applicationContext.RealmId);
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;
    IEnumerable<Session> sessions = await _sessionRepository.LoadActiveAsync(user.Id, cancellationToken);
    foreach (Session session in sessions)
    {
      session.SignOut(actorId);
    }

    await _sessionRepository.SaveAsync(sessions, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
