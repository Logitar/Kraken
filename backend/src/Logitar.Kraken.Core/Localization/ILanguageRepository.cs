using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Localization;

public interface ILanguageRepository
{
  Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<Language?> LoadAsync(LanguageId id, long? version, CancellationToken cancellationToken = default);
  Task<Language?> LoadAsync(LanguageId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<Language?> LoadAsync(LanguageId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Language>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Language>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<Language?> LoadAsync(RealmId? realmId, Locale locale, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Language>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);
  Task<Language> LoadDefaultAsync(RealmId? realmId, CancellationToken cancellationToken = default);

  Task SaveAsync(Language language, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken = default);
}
