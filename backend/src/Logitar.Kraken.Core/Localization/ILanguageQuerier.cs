using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Core.Localization;

public interface ILanguageQuerier
{
  Task<LanguageId> FindDefaultIdAsync(CancellationToken cancellationToken = default);

  Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(string localeCode, CancellationToken cancellationToken = default);

  Task<LanguageModel> ReadDefaultAsync(CancellationToken cancellationToken = default);

  Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken = default);
}
