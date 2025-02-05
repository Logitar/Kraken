using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.Core.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class DictionariesTests
{
  [Theory(DisplayName = "It should construct the correct instance from arguments.")]
  [InlineData("fr")]
  [InlineData("fr-CA")]
  public void Given_Arguments_When_ctor_Then_Constructed(string targetLocaleValue)
  {
    Assert.NotEqual("en", targetLocaleValue.Trim().ToLowerInvariant());
    Dictionary<Locale, Dictionary> dictionariesByLocale = new(capacity: 3);

    Locale defaultLocale = new("en");
    Language defaultLanguage = new(defaultLocale, isDefault: true);
    Dictionary defaultDictionary = new(defaultLanguage);
    dictionariesByLocale[defaultLocale] = defaultDictionary;

    Locale targetLocale = new(targetLocaleValue);
    Language targetLanguage = new(targetLocale);
    Dictionary targetDictionary = new(targetLanguage);
    dictionariesByLocale[targetLocale] = targetDictionary;

    Locale? fallbackLocale = null;
    Language? fallbackLanguage;
    Dictionary? fallbackDictionary = null;
    int index = targetLocaleValue.IndexOf('-');
    if (index >= 0)
    {
      fallbackLocale = new(targetLocaleValue[..index]);
      fallbackLanguage = new(fallbackLocale);
      fallbackDictionary = new(fallbackLanguage);
      dictionariesByLocale[fallbackLocale] = fallbackDictionary;
    }

    Dictionaries dictionaries = new(dictionariesByLocale, targetLocale, defaultLocale);

    Assert.Equal(defaultDictionary, dictionaries.DefaultDictionary);
    Assert.Equal(defaultLocale, dictionaries.DefaultLocale);
    Assert.Equal(targetDictionary, dictionaries.TargetDictionary);
    Assert.Equal(targetLocale, dictionaries.TargetLocale);
    Assert.Equal(fallbackDictionary, dictionaries.FallbackDictionary);
    Assert.Equal(fallbackLocale, dictionaries.FallbackLocale);
  }

  [Fact(DisplayName = "It should construct the correct instance from default arguments.")]
  public void Given_DefaultArguments_When_ctor_Then_Constructed()
  {
    Dictionaries dictionaries = new();
    Assert.Null(dictionaries.DefaultDictionary);
    Assert.Null(dictionaries.DefaultLocale);
    Assert.Null(dictionaries.FallbackDictionary);
    Assert.Null(dictionaries.FallbackLocale);
    Assert.Null(dictionaries.TargetDictionary);
    Assert.Null(dictionaries.TargetLocale);

    dictionaries = new(new Dictionary<Locale, Dictionary>());
    Assert.Null(dictionaries.DefaultDictionary);
    Assert.Null(dictionaries.DefaultLocale);
    Assert.Null(dictionaries.FallbackDictionary);
    Assert.Null(dictionaries.FallbackLocale);
    Assert.Null(dictionaries.TargetDictionary);
    Assert.Null(dictionaries.TargetLocale);
  }

  [Fact(DisplayName = "Translate: it should return the key when the translation was not found.")]
  public void Given_NotFound_When_Translate_Then_KeyReturned()
  {
    Dictionaries dictionaries = new();
    Identifier key = new("PasswordRecovery");
    Assert.Equal(key.Value, dictionaries.Translate(key));
  }

  [Fact(DisplayName = "Translate: it should return the value when the translation was found.")]
  public void Given_Found_When_Translate_Then_Valueeturned()
  {
    Dictionary<Locale, Dictionary> dictionariesByLocale = new(capacity: 3);

    Locale defaultLocale = new("en");
    Language defaultLanguage = new(defaultLocale, isDefault: true);
    Dictionary defaultDictionary = new(defaultLanguage);
    defaultDictionary.SetEntry(new Identifier("Blue"), "Blue");
    defaultDictionary.SetEntry(new Identifier("Green"), "Green");
    defaultDictionary.SetEntry(new Identifier("Red"), "Red");
    dictionariesByLocale[defaultLocale] = defaultDictionary;

    Locale targetLocale = new("fr-CA");
    Language targetLanguage = new(targetLocale);
    Dictionary targetDictionary = new(targetLanguage);
    targetDictionary.SetEntry(new Identifier("Red"), "Écarlate");
    dictionariesByLocale[targetLocale] = targetDictionary;

    Locale fallbackLocale = new("fr");
    Language fallbackLanguage = new(fallbackLocale);
    Dictionary fallbackDictionary = new(fallbackLanguage);
    fallbackDictionary.SetEntry(new Identifier("Blue"), "Cyan");
    fallbackDictionary.SetEntry(new Identifier("Red"), "Rouge");
    dictionariesByLocale[fallbackLocale] = fallbackDictionary;

    Dictionaries dictionaries = new(dictionariesByLocale, targetLocale, defaultLocale);

    Assert.Equal("Écarlate", dictionaries.Translate("Red"));
    Assert.Equal("Cyan", dictionaries.Translate("Blue"));
    Assert.Equal("Green", dictionaries.Translate(new Identifier("Green")));
  }
}
