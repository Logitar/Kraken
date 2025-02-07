using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class LanguageRepository : Repository, ILanguageRepository
{
  private readonly DbSet<LanguageEntity> _languages;

  public LanguageRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _languages = context.Languages;
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
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Language>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Language>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Language>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<Language?> LoadAsync(RealmId? realmId, Locale locale, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();
    string codeNormalized = Helper.Normalize(locale);

    string? streamId = await _languages.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.CodeNormalized == codeNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<Language>(new StreamId(streamId), cancellationToken);
  }

  public async Task<IReadOnlyCollection<Language>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _languages.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Language>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<Language> LoadDefaultAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string? streamId = await _languages.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return (streamId == null ? null : await LoadAsync<Language>(new StreamId(streamId), cancellationToken))
      ?? throw new InvalidOperationException($"The default language could not be found for realm 'Id={id?.ToString() ?? "<null>"}'.");
  }

  public async Task SaveAsync(Language session, CancellationToken cancellationToken)
  {
    await base.SaveAsync(session, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Language> sessions, CancellationToken cancellationToken)
  {
    await base.SaveAsync(sessions, cancellationToken);
  }
}
