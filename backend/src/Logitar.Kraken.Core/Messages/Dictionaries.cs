using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.Core.Messages;

public record Dictionaries
{
  public Dictionary? Target { get; }
  public Dictionary? Fallback { get; }
  public Dictionary? Default { get; }

  public Dictionaries()
  {
  }

  public Dictionaries(IReadOnlyDictionary<Locale, Dictionary> dictionaries, Locale? targetLocale = null, Locale? defaultLocale = null) : this()
  {
    if (targetLocale != null)
    {
      if (dictionaries.TryGetValue(targetLocale, out Dictionary? target))
      {
        Target = target;
      }

      if (!string.IsNullOrEmpty(targetLocale.Culture.Parent?.Name))
      {
        Locale fallbackLocale = new(targetLocale.Culture.Parent.Name);
        if (dictionaries.TryGetValue(fallbackLocale, out Dictionary? fallback))
        {
          Fallback = fallback;
        }
      }
    }

    if (defaultLocale != null && dictionaries.TryGetValue(defaultLocale, out Dictionary? @default))
    {
      Default = @default;
    }
  }

  public string Translate(string key) => Translate(new Identifier(key));
  public string Translate(Identifier key) => Target?.Translate(key) ?? Fallback?.Translate(key) ?? Default?.Translate(key) ?? key.Value;
}
