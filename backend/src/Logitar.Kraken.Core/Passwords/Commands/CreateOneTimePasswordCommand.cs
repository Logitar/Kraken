using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Passwords;
using Logitar.Kraken.Core.Passwords.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Passwords.Commands;

public record CreateOneTimePasswordCommand(CreateOneTimePasswordPayload Payload) : Activity, IRequest<OneTimePasswordModel>;

internal class CreateOneTimePasswordCommandHandler : IRequestHandler<CreateOneTimePasswordCommand, OneTimePasswordModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IPasswordManager _passwordManager;

  public CreateOneTimePasswordCommandHandler(
    IApplicationContext applicationContext,
    IOneTimePasswordQuerier oneTimePasswordQuerier,
    IOneTimePasswordRepository oneTimePasswordRepository,
    IPasswordManager passwordManager)
  {
    _applicationContext = applicationContext;
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
    _passwordManager = passwordManager;
  }

  public async Task<OneTimePasswordModel> Handle(CreateOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    CreateOneTimePasswordPayload payload = command.Payload;
    new CreateOneTimePasswordValidator().ValidateAndThrow(payload);

    OneTimePasswordId oneTimePasswordId = OneTimePasswordId.NewId(_applicationContext.RealmId);
    OneTimePassword? oneTimePassword;
    if (payload.Id.HasValue)
    {
      oneTimePasswordId = new(oneTimePasswordId.RealmId, payload.Id.Value);
      oneTimePassword = await _oneTimePasswordRepository.LoadAsync(oneTimePasswordId, cancellationToken);
      if (oneTimePassword != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value, nameof(payload.Id));
      }
    }

    ActorId? actorId = _applicationContext.ActorId;

    Password password = _passwordManager.Generate(payload.Characters, payload.Length, out string passwordString);
    oneTimePassword = new(password, payload.ExpiresOn, payload.MaximumAttempts, actorId, oneTimePasswordId);

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
