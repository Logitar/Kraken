using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class LanguageQuerier : ILanguageQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<LanguageEntity> _languages;

  public LanguageQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _languages = context.Languages;
  }

  public async Task<LanguageId> FindDefaultIdAsync(CancellationToken cancellationToken)
  {
    string? streamId = await _languages.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken)
      ?? throw new InvalidOperationException($"The default language entity could not be found for realm 'Id={_applicationContext.RealmId?.Value ?? "<null>"}'.");

    return new LanguageId(new StreamId(streamId));
  }
  public async Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken)
  {
    string codeNormalized = Helper.Normalize(locale);

    string? streamId = await _languages.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.CodeNormalized == codeNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new LanguageId(new StreamId(streamId));
  }

  public async Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken) ?? throw new InvalidOperationException($"The language entity 'StreamId={language.Id}' could not be found.");
  }
  public async Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new LanguageId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _languages.AsNoTracking()
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }
  public async Task<LanguageModel?> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    string codeNormalized = Helper.Normalize(locale);

    LanguageEntity? language = await _languages.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.CodeNormalized == codeNormalized, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  public async Task<LanguageModel> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _languages.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.IsDefault, cancellationToken)
      ?? throw new InvalidOperationException($"The default language entity could not be found for realm 'Id={_applicationContext.RealmId?.Value ?? "<null>"}'.");

    return await MapAsync(language, cancellationToken);
  }

  public Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<LanguageModel> MapAsync(LanguageEntity language, CancellationToken cancellationToken)
  {
    return (await MapAsync([language], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LanguageModel>> MapAsync(IEnumerable<LanguageEntity> languages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = languages.SelectMany(language => language.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return languages.Select(language => mapper.ToLanguage(language, _applicationContext.Realm)).ToArray();
  }
}
