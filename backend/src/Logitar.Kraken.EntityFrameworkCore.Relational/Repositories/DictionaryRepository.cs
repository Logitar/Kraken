﻿using Logitar.EventSourcing;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class DictionaryRepository : Repository, IDictionaryRepository
{
  private readonly DbSet<DictionaryEntity> _dictionaries;

  public DictionaryRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _dictionaries = context.Dictionaries;
  }

  public async Task<Dictionary?> LoadAsync(DictionaryId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Dictionary?> LoadAsync(DictionaryId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Dictionary?> LoadAsync(DictionaryId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Dictionary?> LoadAsync(DictionaryId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Dictionary>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Dictionary>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Dictionary>(cancellationToken);
  }
  public async Task<IReadOnlyCollection<Dictionary>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Dictionary>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Dictionary>> LoadAsync(IEnumerable<DictionaryId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Dictionary>> LoadAsync(IEnumerable<DictionaryId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Dictionary>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<Dictionary?> LoadAsync(LanguageId languageId, CancellationToken cancellationToken)
  {
    string? streamId = await _dictionaries.AsNoTracking()
      .WhereRealm(languageId.RealmId)
      .Where(x => x.Language!.Id == languageId.EntityId)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync(new DictionaryId(new StreamId(streamId)), cancellationToken);
  }

  public async Task<IReadOnlyCollection<Dictionary>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _dictionaries.AsNoTracking()
      .WhereRealm(realmId)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new DictionaryId(new StreamId(streamId))), cancellationToken);
  }

  public async Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken)
  {
    await base.SaveAsync(dictionary, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Dictionary> dictionaries, CancellationToken cancellationToken)
  {
    await base.SaveAsync(dictionaries, cancellationToken);
  }
}
