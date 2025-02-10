using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class PublishedContentQuerier : IPublishedContentQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<PublishedContentEntity> _publishedContents;
  private readonly IQueryHelper _queryHelper;

  public PublishedContentQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _publishedContents = context.PublishedContents;
    _queryHelper = queryHelper;
  }

  public async Task<PublishedContent?> ReadAsync(int contentId, CancellationToken cancellationToken)
  {
    PublishedContentEntity[] locales = await _publishedContents.AsNoTracking()
     .WhereRealm(_applicationContext.RealmId)
     .Where(x => x.ContentId == contentId)
     .ToArrayAsync(cancellationToken);

    if (locales.Length < 1)
    {
      return null;
    }

    return (await MapContentsAsync(locales, cancellationToken)).Single();
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentId, CancellationToken cancellationToken)
  {
    PublishedContentEntity[] locales = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentUid == contentId)
      .ToArrayAsync(cancellationToken);

    if (locales.Length < 1)
    {
      return null;
    }

    return (await MapContentsAsync(locales, cancellationToken)).Single();
  }

  public async Task<PublishedContent?> ReadAsync(PublishedContentKey key, CancellationToken cancellationToken)
  {
    if (int.TryParse(key.ContentType, out int contentTypeId))
    {
      if (key.Language != null && int.TryParse(key.Language, out int languageId))
      {
        return await ReadAsync(contentTypeId, languageId, key.UniqueName, cancellationToken);
      }
      else if (key.Language != null && Guid.TryParse(key.Language, out Guid languageUid))
      {
        return await ReadAsync(contentTypeId, languageUid, key.UniqueName, cancellationToken);
      }
      else
      {
        return await ReadAsync(contentTypeId, key.Language, key.UniqueName, cancellationToken);
      }
    }
    else if (Guid.TryParse(key.ContentType, out Guid contentTypeUid))
    {
      if (key.Language != null && int.TryParse(key.Language, out int languageId))
      {
        return await ReadAsync(contentTypeUid, languageId, key.UniqueName, cancellationToken);
      }
      else if (key.Language != null && Guid.TryParse(key.Language, out Guid languageUid))
      {
        return await ReadAsync(contentTypeUid, languageUid, key.UniqueName, cancellationToken);
      }
      else
      {
        return await ReadAsync(contentTypeUid, key.Language, key.UniqueName, cancellationToken);
      }
    }
    else
    {
      if (key.Language != null && int.TryParse(key.Language, out int languageId))
      {
        return await ReadAsync(key.ContentType, languageId, key.UniqueName, cancellationToken);
      }
      else if (key.Language != null && Guid.TryParse(key.Language, out Guid languageUid))
      {
        return await ReadAsync(key.ContentType, languageUid, key.UniqueName, cancellationToken);
      }
      else
      {
        return await ReadAsync(key.ContentType, key.Language, key.UniqueName, cancellationToken);
      }
    }
  }

  public async Task<PublishedContent?> ReadAsync(int contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(string contentTypeName, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(int contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(string contentTypeName, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(int contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    if (languageCode != null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    if (languageCode != null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public async Task<PublishedContent?> ReadAsync(string contentTypeName, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    if (languageCode != null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await _publishedContents.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }

  public async Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(PublishedContents.Table).SelectAll(PublishedContents.Table)
      .WhereRealm(PublishedContents.RealmUid, _applicationContext.RealmId);
    _queryHelper.ApplyTextSearch(builder, payload.Search, PublishedContents.UniqueName, PublishedContents.DisplayName);

    if (payload.Content.Ids.Count > 0)
    {
      object[] contentIds = payload.Content.Ids.Select(id => (object)id).ToArray();
      builder.Where(PublishedContents.ContentId, Operators.IsIn(contentIds));
    }
    if (payload.Content.Uids.Count > 0)
    {
      object[] contentUids = payload.Content.Uids.Select(id => (object)id).ToArray();
      builder.Where(PublishedContents.ContentUid, Operators.IsIn(contentUids));
    }

    if (payload.ContentType.Ids.Count > 0)
    {
      object[] contentTypeIds = payload.ContentType.Ids.Select(id => (object)id).ToArray();
      builder.Where(PublishedContents.ContentTypeId, Operators.IsIn(contentTypeIds));
    }
    if (payload.ContentType.Uids.Count > 0)
    {
      object[] contentTypeUids = payload.ContentType.Uids.Select(id => (object)id).ToArray();
      builder.Where(PublishedContents.ContentTypeUid, Operators.IsIn(contentTypeUids));
    }
    if (payload.ContentType.Names.Count > 0)
    {
      string[] contentTypeNames = payload.ContentType.Names.Select(Helper.Normalize).ToArray();
      builder.Where(PublishedContents.ContentTypeName, Operators.IsIn(contentTypeNames));
    }

    if (payload.Language.Ids.Count > 0)
    {
      object[] languageIds = payload.Language.Ids.Select(id => (object)id).ToArray();
      builder.Where(PublishedContents.LanguageId, Operators.IsIn(languageIds));
    }
    if (payload.Language.Uids.Count > 0)
    {
      object[] languageUids = payload.Language.Uids.Select(id => (object)id).ToArray();
      builder.Where(PublishedContents.LanguageUid, Operators.IsIn(languageUids));
    }
    if (payload.Language.Codes.Count > 0)
    {
      string[] languageCodes = payload.Language.Codes.Select(Helper.Normalize).ToArray();
      builder.Where(PublishedContents.LanguageCode, Operators.IsIn(languageCodes));
    }

    IQueryable<PublishedContentEntity> query = _publishedContents.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<PublishedContentEntity>? ordered = null;
    foreach (PublishedContentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case PublishedContentSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case PublishedContentSort.PublishedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PublishedOn) : query.OrderBy(x => x.PublishedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PublishedOn) : ordered.ThenBy(x => x.PublishedOn));
          break;
        case PublishedContentSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload.Skip, payload.Limit);

    PublishedContentEntity[] publishedContents = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<PublishedContentLocale> items = await MapLocalesAsync(publishedContents, cancellationToken);

    return new SearchResults<PublishedContentLocale>(items, total);
  }

  private async Task<IReadOnlyCollection<PublishedContent>> MapContentsAsync(IEnumerable<PublishedContentEntity> locales, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = locales.SelectMany(locale => locale.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Dictionary<int, ContentTypeSummary> contentTypes = [];
    Dictionary<int, LanguageSummary> languages = [];
    Dictionary<int, PublishedContent> publishedContents = [];
    foreach (PublishedContentEntity locale in locales)
    {
      if (!contentTypes.TryGetValue(locale.ContentTypeId, out ContentTypeSummary? contentType))
      {
        contentType = new(locale.ContentTypeUid, locale.ContentTypeName);
        contentTypes[locale.ContentTypeId] = contentType;
      }

      if (!publishedContents.TryGetValue(locale.ContentId, out PublishedContent? publishedContent))
      {
        publishedContent = new()
        {
          Id = locale.ContentUid,
          ContentType = contentType
        };
        publishedContents[locale.ContentId] = publishedContent;
      }

      LanguageSummary? language = null;
      if (locale.LanguageId.HasValue && !languages.TryGetValue(locale.LanguageId.Value, out language))
      {
        language = new()
        {
          IsDefault = locale.LanguageIsDefault
        };
        if (locale.LanguageUid.HasValue)
        {
          language.Id = locale.LanguageUid.Value;
        }
        if (locale.LanguageCode != null)
        {
          language.Locale = new(locale.LanguageCode);
        }
        languages[locale.LanguageId.Value] = language;
      }

      PublishedContentLocale publishedLocale = mapper.ToPublishedContentLocale(locale, publishedContent, language);
      if (language == null)
      {
        publishedContent.Invariant = publishedLocale;
      }
      else
      {
        publishedContent.Locales.Add(publishedLocale);
      }
    }

    return publishedContents.Values;
  }

  private async Task<IReadOnlyCollection<PublishedContentLocale>> MapLocalesAsync(IEnumerable<PublishedContentEntity> locales, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = locales.SelectMany(locale => locale.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Dictionary<int, ContentTypeSummary> contentTypes = [];
    Dictionary<int, LanguageSummary> languages = [];
    Dictionary<int, PublishedContent> publishedContents = [];
    List<PublishedContentLocale> publishedLocales = [];
    foreach (PublishedContentEntity locale in locales)
    {
      if (!contentTypes.TryGetValue(locale.ContentTypeId, out ContentTypeSummary? contentType))
      {
        contentType = new(locale.ContentTypeUid, locale.ContentTypeName);
        contentTypes[locale.ContentTypeId] = contentType;
      }

      if (!publishedContents.TryGetValue(locale.ContentId, out PublishedContent? publishedContent))
      {
        publishedContent = new()
        {
          Id = locale.ContentUid,
          ContentType = contentType
        };
        publishedContents[locale.ContentId] = publishedContent;
      }

      LanguageSummary? language = null;
      if (locale.LanguageId.HasValue && !languages.TryGetValue(locale.LanguageId.Value, out language))
      {
        language = new()
        {
          IsDefault = locale.LanguageIsDefault
        };
        if (locale.LanguageUid.HasValue)
        {
          language.Id = locale.LanguageUid.Value;
        }
        if (locale.LanguageCode != null)
        {
          language.Locale = new(locale.LanguageCode);
        }
        languages[locale.LanguageId.Value] = language;
      }

      PublishedContentLocale publishedLocale = mapper.ToPublishedContentLocale(locale, publishedContent, language);
      if (language == null)
      {
        publishedContent.Invariant = publishedLocale;
      }
      else
      {
        publishedContent.Locales.Add(publishedLocale);
      }
      publishedLocales.Add(publishedLocale);
    }

    return publishedLocales.AsReadOnly();
  }
}
