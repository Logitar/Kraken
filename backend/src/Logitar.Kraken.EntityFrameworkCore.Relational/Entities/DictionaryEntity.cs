using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Dictionaries.Events;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class DictionaryEntity : AggregateEntity
{
  public int DictionaryId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public Guid Id { get; private set; }

  public LanguageEntity? Language { get; private set; }
  public int LanguageId { get; private set; }

  public int EntryCount { get; private set; }
  public string? Entries { get; private set; }

  public DictionaryEntity(LanguageEntity language, DictionaryCreated @event) : base(@event)
  {
    Realm = language.Realm;
    RealmId = language.RealmId;

    DictionaryId dictionaryId = new(@event.StreamId);
    Id = dictionaryId.EntityId;

    Language = language;
    LanguageId = language.LanguageId;
  }

  private DictionaryEntity() : base()
  {
  }

  public void SetLanguage(LanguageEntity language, DictionaryLanguageChanged @event)
  {
    Update(@event);

    Language = language;
    LanguageId = language.LanguageId;
  }

  public void Update(DictionaryUpdated @event)
  {
    base.Update(@event);

    Dictionary<string, string> entries = GetEntries();
    foreach (KeyValuePair<Identifier, string?> entry in @event.Entries)
    {
      if (entry.Value == null)
      {
        entries.Remove(entry.Key.Value);
      }
      else
      {
        entries[entry.Key.Value] = entry.Value;
      }
    }
    SetEntries(entries);
  }

  public Dictionary<string, string> GetEntries()
  {
    return (Entries == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Entries)) ?? [];
  }
  private void SetEntries(Dictionary<string, string> entries)
  {
    EntryCount = entries.Count;
    Entries = entries.Count < 1 ? null : JsonSerializer.Serialize(entries);
  }
}
