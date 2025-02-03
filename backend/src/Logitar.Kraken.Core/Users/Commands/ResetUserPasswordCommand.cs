using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Users.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Users.Commands;

public record ResetUserPasswordCommand(Guid Id, ResetUserPasswordPayload Payload) : Activity, IRequest<UserModel?>
{
  public override IActivity Anonymize()
  {
    ResetUserPasswordCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, UserModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ResetUserPasswordCommandHandler(
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    IUserManager userManager,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _applicationContext.UserSettings;

    ResetUserPasswordPayload payload = command.Payload;
    new ResetUserPasswordValidator(userSettings.Password).ValidateAndThrow(payload);

    UserId userId = new(_applicationContext.RealmId, command.Id);
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }
    ActorId actorId = new(user.Id.Value);

    Password password = _passwordManager.ValidateAndCreate(userSettings.Password, payload.Password);
    user.ResetPassword(password, actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
