using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.Core.Messages;

public record Dictionaries
{
  public Dictionary? DefaultDictionary { get; }
  public Locale? DefaultLocale { get; }

  public Dictionary? FallbackDictionary { get; }
  public Locale? FallbackLocale { get; }

  public Dictionary? TargetDictionary { get; }
  public Locale? TargetLocale { get; }

  public Dictionaries()
  {
  }

  public Dictionaries(IReadOnlyDictionary<Locale, Dictionary> dictionaries, Locale? targetLocale = null, Locale? defaultLocale = null) : this()
  {
    if (targetLocale != null)
    {
      TargetLocale = targetLocale;
      if (dictionaries.TryGetValue(targetLocale, out Dictionary? target))
      {
        TargetDictionary = target;
      }

      if (!string.IsNullOrEmpty(targetLocale.Culture.Parent?.Name))
      {
        Locale fallbackLocale = new(targetLocale.Culture.Parent.Name);
        FallbackLocale = fallbackLocale;
        if (dictionaries.TryGetValue(fallbackLocale, out Dictionary? fallback))
        {
          FallbackDictionary = fallback;
        }
      }
    }

    if (defaultLocale != null && dictionaries.TryGetValue(defaultLocale, out Dictionary? @default))
    {
      DefaultLocale = defaultLocale;
      DefaultDictionary = @default;
    }
  }

  public string Translate(string key) => Translate(new Identifier(key));
  public string Translate(Identifier key) => TargetDictionary?.Translate(key) ?? FallbackDictionary?.Translate(key) ?? DefaultDictionary?.Translate(key) ?? key.Value;
}
