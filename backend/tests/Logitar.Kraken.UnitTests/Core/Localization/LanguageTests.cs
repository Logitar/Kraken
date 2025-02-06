using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization.Events;

namespace Logitar.Kraken.Core.Localization;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageTests
{
  private readonly Language _language = new(new Locale("en"));

  [Fact(DisplayName = "Delete: it should delete the language.")]
  public void Given_Language_When_Delete_Then_Deleted()
  {
    _language.Delete();
    Assert.True(_language.IsDeleted);

    _language.ClearChanges();
    _language.Delete();
    Assert.False(_language.HasChanges);
    Assert.Empty(_language.Changes);
  }

  [Fact(DisplayName = "It should construct the correct instance.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    Locale locale = new("en");
    bool isDefault = true;
    ActorId actorId = ActorId.NewId();
    LanguageId languageId = LanguageId.NewId(realmId: null);

    Language language = new(locale, isDefault, actorId, languageId);

    Assert.Equal(languageId, language.Id);
    Assert.Equal(actorId, language.CreatedBy);
    Assert.Equal(actorId, language.UpdatedBy);
    Assert.Equal(isDefault, language.IsDefault);
    Assert.Same(locale, language.Locale);
  }

  [Fact(DisplayName = "SetDefault: it should set the language default or not.")]
  public void Given_Language_When_SetDefault_Then_DefaultSet()
  {
    Assert.False(_language.IsDefault);

    ActorId actorId = ActorId.NewId();
    _language.SetDefault(isDefault: true, actorId);
    Assert.True(_language.IsDefault);
    Assert.Contains(_language.Changes, change => change is LanguageSetDefault set && set.IsDefault && set.ActorId == actorId);

    _language.ClearChanges();
    _language.SetDefault();
    Assert.False(_language.HasChanges);
    Assert.Empty(_language.Changes);

    _language.SetDefault(isDefault: false, actorId);
    Assert.False(_language.IsDefault);
    Assert.Contains(_language.Changes, change => change is LanguageSetDefault set && !set.IsDefault && set.ActorId == actorId);
  }

  [Fact(DisplayName = "SetLocale: it should set the language locale.")]
  public void Given_Language_When_SetLocale_Then_LocaleChanged()
  {
    _language.ClearChanges();
    _language.SetLocale(_language.Locale);
    Assert.False(_language.HasChanges);
    Assert.Empty(_language.Changes);

    Locale locale = new("fr");
    ActorId actorId = ActorId.NewId();
    _language.SetLocale(locale, actorId);
    Assert.Same(locale, _language.Locale);
    Assert.Contains(_language.Changes, change => change is LanguageLocaleChanged changed && changed.ActorId == actorId && changed.Locale.Equals(locale));
  }

  [Fact(DisplayName = "ToString: it should return the correct representation.")]
  public void Given_Language_When_ToString_Then_CorrectRepresentation()
  {
    Assert.StartsWith(_language.Locale.ToString(), _language.ToString());
  }
}
