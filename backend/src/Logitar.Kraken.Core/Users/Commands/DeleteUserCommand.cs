using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Sessions;
using MediatR;

namespace Logitar.Kraken.Core.Users.Commands;

public record DeleteUserCommand(Guid Id) : Activity, IRequest<UserModel?>;

internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, UserModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public DeleteUserCommandHandler(
    IApplicationContext applicationContext,
    IOneTimePasswordRepository oneTimePasswordRepository,
    ISessionRepository sessionRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _sessionRepository = sessionRepository;
    _oneTimePasswordRepository = oneTimePasswordRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
  {
    UserId userId = new(command.Id, _applicationContext.RealmId);
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }
    UserModel result = await _userQuerier.ReadAsync(user, cancellationToken);

    ActorId? actorId = _applicationContext.ActorId;

    IReadOnlyCollection<OneTimePassword> oneTimePasswords = await _oneTimePasswordRepository.LoadAsync(user.Id, cancellationToken);
    foreach (OneTimePassword oneTimePassword in oneTimePasswords)
    {
      oneTimePassword.Delete(actorId);
    }
    await _oneTimePasswordRepository.SaveAsync(oneTimePasswords, cancellationToken);

    IReadOnlyCollection<Session> sessions = await _sessionRepository.LoadAsync(user.Id, cancellationToken);
    foreach (Session session in sessions)
    {
      session.Delete(actorId);
    }
    await _sessionRepository.SaveAsync(sessions, cancellationToken);

    user.Delete(actorId);
    await _userRepository.SaveAsync(user, cancellationToken);

    return result;
  }
}
