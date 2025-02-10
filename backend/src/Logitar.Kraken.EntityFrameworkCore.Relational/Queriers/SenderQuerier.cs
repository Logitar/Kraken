using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class SenderQuerier : ISenderQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<SenderEntity> _senders;

  public SenderQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _senders = context.Senders;
  }

  public async Task<SenderId?> FindDefaultIdAsync(SenderType type, CancellationToken cancellationToken)
  {
    string? streamId = await _senders.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.Type == type && x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new SenderId(new StreamId(streamId));
  }

  public async Task<SenderModel> ReadAsync(Sender sender, CancellationToken cancellationToken)
  {
    return await ReadAsync(sender.Id, cancellationToken) ?? throw new InvalidOperationException($"The sender entity 'StreamId={sender.Id}' could not be found.");
  }
  public async Task<SenderModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new SenderId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<SenderModel?> ReadAsync(SenderId id, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _senders.AsNoTracking()
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return sender == null ? null : await MapAsync(sender, cancellationToken);
  }

  public async Task<SenderModel?> ReadDefaultAsync(SenderType type, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _senders.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.Type == type && x.IsDefault)
      .SingleOrDefaultAsync(cancellationToken);

    return sender == null ? null : await MapAsync(sender, cancellationToken);
  }

  public Task<SearchResults<SenderModel>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<SenderModel> MapAsync(SenderEntity sender, CancellationToken cancellationToken)
  {
    return (await MapAsync([sender], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<SenderModel>> MapAsync(IEnumerable<SenderEntity> senders, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = senders.SelectMany(sender => sender.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return senders.Select(sender => mapper.ToSender(sender, _applicationContext.Realm)).ToArray();
  }
}
