using FluentValidation;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Core.Localization.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Localization.Commands;

/// <exception cref="LocaleAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateLanguageCommand(Guid Id, UpdateLanguagePayload Payload) : IRequest<LanguageModel?>;

internal class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, LanguageModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILanguageManager _languageManager;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public UpdateLanguageCommandHandler(
    IApplicationContext applicationContext,
    ILanguageManager languageManager,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository)
  {
    _applicationContext = applicationContext;
    _languageManager = languageManager;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task<LanguageModel?> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
  {
    UpdateLanguagePayload payload = command.Payload;
    new UpdateLanguageValidator().ValidateAndThrow(payload);

    LanguageId languageId = new(command.Id, _applicationContext.RealmId);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language == null)
    {
      return null;
    }

    Locale? locale = Locale.TryCreate(payload.Locale);
    if (locale != null)
    {
      language.SetLocale(locale, _applicationContext.ActorId);
    }

    await _languageManager.SaveAsync(language, cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
