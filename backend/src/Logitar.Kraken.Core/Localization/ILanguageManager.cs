namespace Logitar.Kraken.Core.Localization;

public interface ILanguageManager
{
  Task SaveAsync(Language language, CancellationToken cancellationToken = default);
}
