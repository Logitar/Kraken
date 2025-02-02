using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Users.Commands;

public record RemoveUserIdentifierCommand(Guid Id, string Key) : Activity, IRequest<UserModel?>;

internal class RemoveUserIdentifierCommandHandler : IRequestHandler<RemoveUserIdentifierCommand, UserModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public RemoveUserIdentifierCommandHandler(
    IApplicationContext applicationContext,
    IUserManager userManager,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(RemoveUserIdentifierCommand command, CancellationToken cancellationToken)
  {
    CustomIdentifierModel identifier = new(command.Key, value: "Temporary");
    new CustomIdentifierValidator().ValidateAndThrow(identifier);

    UserId userId = new(_applicationContext.RealmId, command.Id);
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    Identifier key = new(command.Key);
    ActorId? actorId = _applicationContext.ActorId;
    user.RemoveCustomIdentifier(key, actorId);
    await _userManager.SaveAsync(_applicationContext.UserSettings, user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
