using Logitar.Kraken.Contracts.Localization;

namespace Logitar.Kraken.Core.Localization;

public interface ILanguageQuerier
{
  Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(string localeCode, CancellationToken cancellationToken = default);
}
