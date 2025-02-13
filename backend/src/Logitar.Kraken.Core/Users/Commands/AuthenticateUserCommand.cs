using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Users.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Users.Commands;

// TODO(fpion): UserManager exceptions
/// <exception cref="IncorrectUserPasswordException"></exception>
/// <exception cref="UserHasNoPasswordException"></exception>
/// <exception cref="UserIsDisabledException"></exception>
/// <exception cref="ValidationException"></exception>
public record AuthenticateUserCommand(AuthenticateUserPayload Payload) : Activity, IRequest<UserModel>
{
  public override IActivity Anonymize()
  {
    AuthenticateUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserModel>
{
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public AuthenticateUserCommandHandler(IUserManager userManager, IUserQuerier userQuerier)
  {
    _userManager = userManager;
    _userQuerier = userQuerier;
  }

  public async Task<UserModel> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    User user = await _userManager.FindAsync(payload.UniqueName, nameof(payload.UniqueName), includeId: false, cancellationToken);

    ActorId actorId = new(user.Id.Value);
    user.Authenticate(payload.Password, actorId);

    await _userManager.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
