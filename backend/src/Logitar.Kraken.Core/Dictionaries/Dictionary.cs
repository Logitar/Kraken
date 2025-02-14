using Logitar.EventSourcing;
using Logitar.Kraken.Core.Dictionaries.Events;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Dictionaries;

public class Dictionary : AggregateRoot
{
  private DictionaryUpdated _updated = new();

  public new DictionaryId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private LanguageId? _languageId = null;
  public LanguageId LanguageId => _languageId ?? throw new InvalidOperationException("The dictionary has not been initialized.");

  private readonly Dictionary<Identifier, string> _entries = [];
  public IReadOnlyDictionary<Identifier, string> Entries => _entries.AsReadOnly();

  public Dictionary() : base()
  {
  }

  public Dictionary(Language language, ActorId? actorId = null, DictionaryId? dictionaryId = null) : base(dictionaryId?.StreamId)
  {
    if (RealmId != language.RealmId)
    {
      throw new RealmMismatchException(RealmId, language.RealmId, nameof(language));
    }

    Raise(new DictionaryCreated(language.Id), actorId);
  }
  protected virtual void Handle(DictionaryCreated @event)
  {
    _languageId = @event.LanguageId;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new DictionaryDeleted(), actorId);
    }
  }

  public void RemoveEntry(Identifier key)
  {
    if (_entries.Remove(key))
    {
      _updated.Entries[key] = null;
    }
  }

  public void SetEntry(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveEntry(key);
    }
    else
    {
      value = value.Trim();
      if (!_entries.TryGetValue(key, out string? existingValue) || existingValue != value)
      {
        _updated.Entries[key] = value;
        _entries[key] = value;
      }
    }
  }

  public void SetLanguage(Language language, ActorId? actorId = null)
  {
    if (RealmId != language.RealmId)
    {
      throw new RealmMismatchException(RealmId, language.RealmId, nameof(language));
    }

    if (_languageId != language.Id)
    {
      Raise(new DictionaryLanguageChanged(language.Id), actorId);
    }
  }
  protected virtual void Handle(DictionaryLanguageChanged @event)
  {
    _languageId = @event.LanguageId;
  }

  public string? Translate(Identifier key) => _entries.TryGetValue(key, out string? value) ? value : null;

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(DictionaryUpdated @event)
  {
    foreach (KeyValuePair<Identifier, string?> entry in @event.Entries)
    {
      if (entry.Value == null)
      {
        _entries.Remove(entry.Key);
      }
      else
      {
        _entries[entry.Key] = entry.Value;
      }
    }
  }
}
