using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Users.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Users.Commands;

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
  private readonly IApplicationContext _applicationContext;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public AuthenticateUserCommandHandler(IApplicationContext applicationContext, IUserManager userManager, IUserQuerier userQuerier)
  {
    _applicationContext = applicationContext;
    _userManager = userManager;
    _userQuerier = userQuerier;
  }

  public async Task<UserModel> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    IUserSettings userSettings = _applicationContext.UserSettings;

    User user = await _userManager.FindAsync(userSettings, realmId, payload.UniqueName, nameof(payload.UniqueName), includeId: false, cancellationToken);
    ActorId actorId = new(user.Id.Value);

    user.Authenticate(payload.Password, actorId);

    await _userManager.SaveAsync(userSettings, user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
