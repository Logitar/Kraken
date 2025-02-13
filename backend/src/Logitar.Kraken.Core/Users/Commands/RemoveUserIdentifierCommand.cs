using FluentValidation;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Users.Commands;

/// <exception cref="ValidationException"></exception>
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

    UserId userId = new(command.Id, _applicationContext.RealmId);
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    Identifier key = new(command.Key);
    user.RemoveCustomIdentifier(key, _applicationContext.ActorId);
    await _userManager.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
