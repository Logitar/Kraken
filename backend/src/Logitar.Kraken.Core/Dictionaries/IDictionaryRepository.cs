using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Dictionaries;

public interface IDictionaryRepository
{
  Task<Dictionary?> LoadAsync(DictionaryId id, CancellationToken cancellationToken = default);
  Task<Dictionary?> LoadAsync(DictionaryId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<Dictionary?> LoadAsync(DictionaryId id, long? version, CancellationToken cancellationToken = default);
  Task<Dictionary?> LoadAsync(DictionaryId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Dictionary>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Dictionary>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Dictionary>> LoadAsync(IEnumerable<DictionaryId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Dictionary>> LoadAsync(IEnumerable<DictionaryId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<Dictionary?> LoadAsync(LanguageId languageId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Dictionary>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);

  Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Dictionary> dictionaries, CancellationToken cancellationToken = default);
}
