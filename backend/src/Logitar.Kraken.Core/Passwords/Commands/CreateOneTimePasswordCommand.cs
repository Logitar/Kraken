using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Passwords;
using Logitar.Kraken.Core.Passwords.Validators;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Passwords.Commands;

public record CreateOneTimePasswordCommand(CreateOneTimePasswordPayload Payload) : Activity, IRequest<OneTimePasswordModel>;

internal class CreateOneTimePasswordCommandHandler : IRequestHandler<CreateOneTimePasswordCommand, OneTimePasswordModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;

  public CreateOneTimePasswordCommandHandler(
    IApplicationContext applicationContext,
    IOneTimePasswordQuerier oneTimePasswordQuerier,
    IOneTimePasswordRepository oneTimePasswordRepository,
    IPasswordManager passwordManager,
    IUserManager userManager)
  {
    _applicationContext = applicationContext;
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
    _passwordManager = passwordManager;
    _userManager = userManager;
  }

  public async Task<OneTimePasswordModel> Handle(CreateOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    CreateOneTimePasswordPayload payload = command.Payload;
    new CreateOneTimePasswordValidator().ValidateAndThrow(payload);

    ActorId? actorId = _applicationContext.ActorId;
    RealmId? realmId = _applicationContext.RealmId;

    OneTimePasswordId oneTimePasswordId = OneTimePasswordId.NewId(realmId);
    OneTimePassword? oneTimePassword;
    if (payload.Id.HasValue)
    {
      oneTimePasswordId = new(realmId, payload.Id.Value);
      oneTimePassword = await _oneTimePasswordRepository.LoadAsync(oneTimePasswordId, cancellationToken);
      if (oneTimePassword != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value, nameof(payload.Id));
      }
    }

    Password password = _passwordManager.Generate(payload.Characters, payload.Length, out string passwordString);
    User? user = string.IsNullOrWhiteSpace(payload.User)
      ? null
      : await _userManager.FindAsync(payload.User, nameof(payload.User), includeId: true, cancellationToken);
    oneTimePassword = new(password, payload.ExpiresOn, payload.MaximumAttempts, user?.Id, actorId, oneTimePasswordId);

    foreach (CustomAttributeModel customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      oneTimePassword.SetCustomAttribute(key, customAttribute.Value);
    }

    oneTimePassword.Update(actorId);
    await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    OneTimePasswordModel model = await _oneTimePasswordQuerier.ReadAsync(oneTimePassword, cancellationToken);
    model.Password = passwordString;
    return model;
  }
}
