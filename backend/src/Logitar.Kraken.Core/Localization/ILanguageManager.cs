namespace Logitar.Kraken.Core.Localization;

public interface ILanguageManager
{
  Task<Language> FindAsync(string language, CancellationToken cancellationToken = default);
  Task SaveAsync(Language language, CancellationToken cancellationToken = default);
}
