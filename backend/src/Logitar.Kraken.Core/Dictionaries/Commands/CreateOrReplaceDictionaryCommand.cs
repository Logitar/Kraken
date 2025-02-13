using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Core.Dictionaries.Validators;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Commands;

public record CreateOrReplaceDictionaryResult(DictionaryModel? Dictionary = null, bool Created = false);

public record CreateOrReplaceDictionaryCommand(Guid? Id, CreateOrReplaceDictionaryPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceDictionaryResult>;

internal class ReplaceDictionaryCommandHandler : IRequestHandler<CreateOrReplaceDictionaryCommand, CreateOrReplaceDictionaryResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryManager _dictionaryManager;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly ILanguageManager _languageManager;

  public ReplaceDictionaryCommandHandler(
    IApplicationContext applicationContext,
    IDictionaryManager dictionaryManager,
    IDictionaryQuerier dictionaryQuerier,
    IDictionaryRepository dictionaryRepository,
    ILanguageManager languageManager)
  {
    _applicationContext = applicationContext;
    _dictionaryManager = dictionaryManager;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
    _languageManager = languageManager;
  }

  public async Task<CreateOrReplaceDictionaryResult> Handle(CreateOrReplaceDictionaryCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceDictionaryPayload payload = command.Payload;
    new CreateOrReplaceDictionaryValidator().ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    DictionaryId dictionaryId = DictionaryId.NewId(realmId);
    Dictionary? dictionary = null;
    if (command.Id.HasValue)
    {
      dictionaryId = new(command.Id.Value, realmId);
      dictionary = await _dictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
    }

    ActorId? actorId = _applicationContext.ActorId;
    Language language = await _languageManager.FindAsync(payload.Language, nameof(payload.Language), cancellationToken);

    bool created = false;
    if (dictionary == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceDictionaryResult();
      }

      dictionary = new(language, actorId, dictionaryId);
      created = true;
    }

    Dictionary reference = (command.Version.HasValue
      ? await _dictionaryRepository.LoadAsync(dictionary.Id, command.Version.Value, cancellationToken)
      : null) ?? dictionary;

    if (reference.LanguageId != language.Id)
    {
      dictionary.SetLanguage(language, actorId);
    }

    SetDictionaryEntries(payload, dictionary, reference);

    dictionary.Update(actorId);
    await _dictionaryManager.SaveAsync(dictionary, cancellationToken);

    DictionaryModel model = await _dictionaryQuerier.ReadAsync(dictionary, cancellationToken);
    return new CreateOrReplaceDictionaryResult(model, created);
  }

  private static void SetDictionaryEntries(CreateOrReplaceDictionaryPayload payload, Dictionary dictionary, Dictionary reference)
  {
    HashSet<Identifier> keys = payload.Entries.Select(entry => new Identifier(entry.Key)).ToHashSet();
    foreach (Identifier key in reference.Entries.Keys)
    {
      if (!keys.Contains(key))
      {
        dictionary.RemoveEntry(key);
      }
    }

    foreach (DictionaryEntryModel entry in payload.Entries)
    {
      Identifier key = new(entry.Key);
      if (!reference.Entries.TryGetValue(key, out string? existingValue) || existingValue != entry.Value)
      {
        dictionary.SetEntry(key, entry.Value);
      }
    }
  }
}
