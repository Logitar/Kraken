using Logitar.EventSourcing;
using Logitar.Kraken.Core.Dictionaries.Events;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Dictionaries;

[Trait(Traits.Category, Categories.Unit)]
public class DictionaryTests
{
  private readonly Language _language = new(new Locale("fr"));
  private readonly Dictionary _dictionary;

  public DictionaryTests()
  {
    _dictionary = new(_language);
  }

  [Fact(DisplayName = "Delete: it should delete the dictionary.")]
  public void Given_Dictionary_When_Delete_Then_Deleted()
  {
    _dictionary.Delete();
    Assert.True(_dictionary.IsDeleted);

    _dictionary.ClearChanges();
    _dictionary.Delete();
    Assert.False(_dictionary.HasChanges);
    Assert.Empty(_dictionary.Changes);
  }

  [Fact(DisplayName = "It should construct the correct instance.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    ActorId actorId = ActorId.NewId();
    RealmId realmId = RealmId.NewId();
    Language language = new(new Locale("en"), isDefault: true, actorId, LanguageId.NewId(realmId));
    Guid entityId = Guid.NewGuid();
    DictionaryId dictionaryId = new(entityId, realmId);

    Dictionary dictionary = new(language, actorId, dictionaryId);

    Assert.Equal(actorId, dictionary.CreatedBy);
    Assert.Equal(actorId, dictionary.UpdatedBy);
    Assert.Equal(dictionaryId, dictionary.Id);
    Assert.Equal(realmId, dictionary.RealmId);
    Assert.Equal(entityId, dictionary.EntityId);
    Assert.Equal(language.Id, dictionary.LanguageId);
    Assert.Empty(dictionary.Entries);
  }

  [Fact(DisplayName = "It should throw RealmMismatchException when the language is in another realm.")]
  public void Given_AnotherRealm_When_ctor_Then_RealmMismatchException()
  {
    Language language = new(_language.Locale, languageId: LanguageId.NewId(RealmId.NewId()));

    var exception = Assert.Throws<RealmMismatchException>(() => new Dictionary(language));
    Assert.Null(exception.ExpectedRealmId);
    Assert.NotNull(language.RealmId);
    Assert.Equal(language.RealmId.Value.ToGuid(), exception.ActualRealmId);
    Assert.Equal("language", exception.PropertyName);
  }

  [Fact(DisplayName = "RemoveEntry: it should remove the entry.")]
  public void Given_Entries_When_RemoveEntry_Then_EntryRemoved()
  {
    Identifier key = new("Blue");
    _dictionary.SetEntry(key, "Bleu");
    _dictionary.Update();

    _dictionary.RemoveEntry(key);
    _dictionary.Update();
    Assert.False(_dictionary.Entries.ContainsKey(key));
    Assert.Contains(_dictionary.Changes, change => change is DictionaryUpdated updated && updated.Entries[key] == null);

    _dictionary.ClearChanges();
    _dictionary.RemoveEntry(key);
    Assert.False(_dictionary.HasChanges);
    Assert.Empty(_dictionary.Changes);
  }

  [Theory(DisplayName = "SetEntry: it should remove the entry when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetEntry_Then_EntryRemoved(string? value)
  {
    Identifier key = new("Green");
    _dictionary.SetEntry(key, "Vert");
    _dictionary.Update();

    _dictionary.SetEntry(key, value!);
    _dictionary.Update();
    Assert.False(_dictionary.Entries.ContainsKey(key));
    Assert.Contains(_dictionary.Changes, change => change is DictionaryUpdated updated && updated.Entries[key] == null);
  }

  [Fact(DisplayName = "SetEntry: it should set an entry.")]
  public void Given_Entry_When_SetEntry_Then_Entrieset()
  {
    Identifier key = new("Yellow");
    string value = "  Jaune  ";

    _dictionary.SetEntry(key, value);
    _dictionary.Update();
    Assert.Equal(_dictionary.Entries[key], value.Trim());
    Assert.Contains(_dictionary.Changes, change => change is DictionaryUpdated updated && updated.Entries[key] == value.Trim());

    _dictionary.ClearChanges();
    _dictionary.SetEntry(key, value.Trim());
    _dictionary.Update();
    Assert.False(_dictionary.HasChanges);
    Assert.Empty(_dictionary.Changes);
  }

  [Fact(DisplayName = "SetLanguage: it should not do anything when it is the same language.")]
  public void Given_NoChange_When_SetLanguage_Then_DoNothing()
  {
    _dictionary.ClearChanges();

    _dictionary.SetLanguage(_language);

    Assert.False(_dictionary.HasChanges);
    Assert.Empty(_dictionary.Changes);
  }

  [Fact(DisplayName = "SetLanguage: it should set the language.")]
  public void Given_Different_When_SetLanguage_Then_LanguageChanged()
  {
    Language language = new(new Locale("fr-CA"));
    ActorId actorId = ActorId.NewId();
    _dictionary.SetLanguage(language, actorId);
    Assert.Equal(language.Id, _dictionary.LanguageId);
    Assert.Contains(_dictionary.Changes, change => change is DictionaryLanguageChanged changed && changed.ActorId == actorId && changed.LanguageId == language.Id);
  }

  [Fact(DisplayName = "SetLanguage: it should throw RealmMismatchException when the language is in another realm.")]
  public void Given_AnotherRealm_When_SetLanguage_Then_RealmMismatchException()
  {
    Language language = new(new Locale("fr-CA"), isDefault: false, actorId: null, LanguageId.NewId(RealmId.NewId()));
    var exception = Assert.Throws<RealmMismatchException>(() => _dictionary.SetLanguage(language));
    Assert.Equal(_dictionary.RealmId?.ToGuid(), exception.ExpectedRealmId);
    Assert.Equal(language.RealmId?.ToGuid(), exception.ActualRealmId);
    Assert.Equal("language", exception.PropertyName);
  }

  [Fact(DisplayName = "Translate: it should return null when the translation was not found.")]
  public void Given_NotFound_When_Translate_Then_NullReturned()
  {
    Identifier key = new("Orange");

    string? value = _dictionary.Translate(key);

    Assert.Null(value);
  }

  [Fact(DisplayName = "Translate: it should return the translation found.")]
  public void Given_Found_When_Translate_Then_TranslationReturned()
  {
    Identifier key = new("Orange");
    string translation = "Orange";
    _dictionary.SetEntry(key, translation);

    string? value = _dictionary.Translate(key);

    Assert.NotNull(value);
    Assert.Equal(translation, value);
  }

  [Theory(DisplayName = "Update: it should update the dictionary.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_DictionaryUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _dictionary.ClearChanges();
    _dictionary.Update();
    Assert.False(_dictionary.HasChanges);
    Assert.Empty(_dictionary.Changes);

    _dictionary.SetEntry(new Identifier("Red"), "Rouge");
    _dictionary.Update(actorId);
    Assert.Contains(_dictionary.Changes, change => change is DictionaryUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }
}
