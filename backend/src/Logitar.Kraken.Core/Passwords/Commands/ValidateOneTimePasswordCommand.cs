using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Passwords;
using Logitar.Kraken.Core.Passwords.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Passwords.Commands;

public record ValidateOneTimePasswordCommand(Guid Id, ValidateOneTimePasswordPayload Payload) : Activity, IRequest<OneTimePasswordModel?>;

internal class ValidateOneTimePasswordCommandHandler : IRequestHandler<ValidateOneTimePasswordCommand, OneTimePasswordModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;

  public ValidateOneTimePasswordCommandHandler(
    IApplicationContext applicationContext,
    IOneTimePasswordQuerier oneTimePasswordQuerier,
    IOneTimePasswordRepository oneTimePasswordRepository)
  {
    _applicationContext = applicationContext;
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
  }

  public async Task<OneTimePasswordModel?> Handle(ValidateOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    ValidateOneTimePasswordPayload payload = command.Payload;
    new ValidateOneTimePasswordValidator().ValidateAndThrow(payload);

    OneTimePasswordId oneTimePasswordId = new(command.Id, _applicationContext.RealmId);
    OneTimePassword? oneTimePassword = await _oneTimePasswordRepository.LoadAsync(oneTimePasswordId, cancellationToken);
    if (oneTimePassword == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    try
    {
      oneTimePassword.Validate(payload.Password, actorId);
    }
    catch (IncorrectOneTimePasswordPasswordException)
    {
      await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);
      throw;
    }

    foreach (CustomAttributeModel customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      oneTimePassword.SetCustomAttribute(key, customAttribute.Value);
    }
    oneTimePassword.Update(actorId);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    return await _oneTimePasswordQuerier.ReadAsync(oneTimePassword, cancellationToken);
  }
}
