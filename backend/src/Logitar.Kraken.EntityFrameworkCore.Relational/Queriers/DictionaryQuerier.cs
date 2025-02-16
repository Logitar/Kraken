using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class DictionaryQuerier : IDictionaryQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<DictionaryEntity> _dictionaries;
  private readonly IQueryHelper _queryHelper;

  public DictionaryQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _dictionaries = context.Dictionaries;
    _queryHelper = queryHelper;
  }

  public async Task<DictionaryId?> FindIdAsync(LanguageId languageId, CancellationToken cancellationToken)
  {
    string? streamId = await _dictionaries.AsNoTracking()
      .Include(x => x.Language)
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.Language!.Id == languageId.EntityId)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new DictionaryId(new StreamId(streamId));
  }

  public async Task<DictionaryModel> ReadAsync(Dictionary dictionary, CancellationToken cancellationToken)
  {
    return await ReadAsync(dictionary.Id, cancellationToken) ?? throw new InvalidOperationException($"The dictionary entity 'StreamId={dictionary.Id}' could not be found.");
  }
  public async Task<DictionaryModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new DictionaryId(id, _applicationContext.RealmId), cancellationToken);
  }
  public async Task<DictionaryModel?> ReadAsync(DictionaryId id, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .Include(x => x.Language)
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return dictionary == null ? null : await MapAsync(dictionary, cancellationToken);
  }
  public async Task<DictionaryModel?> ReadAsync(string language, CancellationToken cancellationToken)
  {
    IQueryable<DictionaryEntity> query = _dictionaries.AsNoTracking()
      .Include(x => x.Language)
      .WhereRealm(_applicationContext.RealmId);
    if (Guid.TryParse(language.Trim(), out Guid id))
    {
      query = query.Where(x => x.Language!.Id == id);
    }
    else
    {
      string codeNormalized = Helper.Normalize(language);
      query = query.Where(x => x.Language!.CodeNormalized == codeNormalized);
    }
    DictionaryEntity? dictionary = await query.SingleOrDefaultAsync(cancellationToken);

    return dictionary == null ? null : await MapAsync(dictionary, cancellationToken);
  }

  public async Task<SearchResults<DictionaryModel>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(Dictionaries.Table).SelectAll(Dictionaries.Table)
      .WhereRealm(Dictionaries.RealmUid, _applicationContext.RealmId)
      .ApplyIdFilter(Dictionaries.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search);

    if (payload.IsEmpty.HasValue)
    {
      ComparisonOperator @operator = payload.IsEmpty.Value ? Operators.IsEqualTo(0) : Operators.IsGreaterThan(0);
      builder.Where(Dictionaries.EntryCount, @operator);
    }

    IQueryable<DictionaryEntity> query = _dictionaries.FromQuery(builder).AsNoTracking()
      .Include(x => x.Language);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<DictionaryEntity>? ordered = null;
    foreach (DictionarySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case DictionarySort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case DictionarySort.EntryCount:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EntryCount) : query.OrderBy(x => x.EntryCount))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EntryCount) : ordered.ThenBy(x => x.EntryCount));
          break;
        case DictionarySort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    DictionaryEntity[] dictionaries = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<DictionaryModel> items = await MapAsync(dictionaries, cancellationToken);

    return new SearchResults<DictionaryModel>(items, total);
  }

  private async Task<DictionaryModel> MapAsync(DictionaryEntity dictionary, CancellationToken cancellationToken)
  {
    return (await MapAsync([dictionary], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<DictionaryModel>> MapAsync(IEnumerable<DictionaryEntity> dictionaries, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = dictionaries.SelectMany(dictionary => dictionary.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return dictionaries.Select(dictionary => mapper.ToDictionary(dictionary, _applicationContext.Realm)).ToArray();
  }
}
