using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Core.Dictionaries.Validators;
using Logitar.Kraken.Core.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Commands;

/// <exception cref="LanguageAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateDictionaryCommand(Guid Id, UpdateDictionaryPayload Payload) : Activity, IRequest<DictionaryModel?>;

internal class UpdateDictionaryCommandHandler : IRequestHandler<UpdateDictionaryCommand, DictionaryModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryManager _dictionaryManager;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly ILanguageManager _languageManager;

  public UpdateDictionaryCommandHandler(
    IApplicationContext applicationContext,
    IDictionaryManager dictionaryManager,
    IDictionaryRepository dictionaryRepository,
    IDictionaryQuerier dictionaryQuerier,
    ILanguageManager languageManager)
  {
    _applicationContext = applicationContext;
    _dictionaryManager = dictionaryManager;
    _dictionaryRepository = dictionaryRepository;
    _dictionaryQuerier = dictionaryQuerier;
    _languageManager = languageManager;
  }

  public async Task<DictionaryModel?> Handle(UpdateDictionaryCommand command, CancellationToken cancellationToken)
  {
    UpdateDictionaryPayload payload = command.Payload;
    new UpdateDictionaryValidator().ValidateAndThrow(payload);

    DictionaryId dictionaryId = new(command.Id, _applicationContext.RealmId);
    Dictionary? dictionary = await _dictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
    if (dictionary == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Language))
    {
      Language language = await _languageManager.FindAsync(payload.Language, nameof(payload.Language), cancellationToken);
      dictionary.SetLanguage(language, actorId);
    }

    foreach (DictionaryEntryModel entry in payload.Entries)
    {
      dictionary.SetEntry(new Identifier(entry.Key), entry.Value);
    }

    dictionary.Update(actorId);
    await _dictionaryManager.SaveAsync(dictionary, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(dictionary, cancellationToken);
  }
}
