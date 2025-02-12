using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class LanguageRepository : Repository, ILanguageRepository
{
  public LanguageRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Language?> LoadAsync(LanguageId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Language?> LoadAsync(LanguageId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Language?> LoadAsync(LanguageId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Language>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Language>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Language>(cancellationToken);
  }
  public async Task<IReadOnlyCollection<Language>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Language>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Language>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(languages, cancellationToken);
  }
}
