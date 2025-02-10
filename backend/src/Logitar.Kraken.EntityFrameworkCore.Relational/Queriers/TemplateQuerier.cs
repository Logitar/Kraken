using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class TemplateQuerier : ITemplateQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<TemplateEntity> _templates;

  public TemplateQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _templates = context.Templates;
  }

  public async Task<TemplateId?> FindIdAsync(Identifier uniqueKey, CancellationToken cancellationToken)
  {
    string uniqueKeyNormalized = Helper.Normalize(uniqueKey);

    string? streamId = await _templates.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.UniqueKeyNormalized == uniqueKeyNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new TemplateId(new StreamId(streamId));
  }

  public async Task<TemplateModel> ReadAsync(Template template, CancellationToken cancellationToken)
  {
    return await ReadAsync(template.Id, cancellationToken) ?? throw new InvalidOperationException($"The template entity 'StreamId={template.Id}' could not be found.");
  }
  public async Task<TemplateModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new TemplateId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<TemplateModel?> ReadAsync(TemplateId id, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await _templates.AsNoTracking()
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return template == null ? null : await MapAsync(template, cancellationToken);
  }
  public async Task<TemplateModel?> ReadAsync(string uniqueKey, CancellationToken cancellationToken)
  {
    string uniqueKeyNormalized = Helper.Normalize(uniqueKey);

    TemplateEntity? template = await _templates.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueKeyNormalized == uniqueKeyNormalized, cancellationToken);

    return template == null ? null : await MapAsync(template, cancellationToken);
  }

  public Task<SearchResults<TemplateModel>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<TemplateModel> MapAsync(TemplateEntity template, CancellationToken cancellationToken)
  {
    return (await MapAsync([template], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<TemplateModel>> MapAsync(IEnumerable<TemplateEntity> templates, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = templates.SelectMany(template => template.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return templates.Select(template => mapper.ToTemplate(template, _applicationContext.Realm)).ToArray();
  }
}
