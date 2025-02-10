using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class SenderRepository : Repository, ISenderRepository
{
  private readonly DbSet<SenderEntity> _senders;

  public SenderRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _senders = context.Senders;
  }

  public async Task<Sender?> LoadAsync(SenderId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Sender?> LoadAsync(SenderId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Sender?> LoadAsync(SenderId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Sender?> LoadAsync(SenderId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Sender>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Sender>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Sender>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Sender>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Sender>> LoadAsync(IEnumerable<SenderId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Sender>> LoadAsync(IEnumerable<SenderId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Sender>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Sender>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _senders.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Sender>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<Sender?> LoadDefaultAsync(RealmId? realmId, SenderType type, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string? streamId = await _senders.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.Type == type && x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<Sender>(new StreamId(streamId), cancellationToken);
  }

  public async Task SaveAsync(Sender sender, CancellationToken cancellationToken)
  {
    await base.SaveAsync(sender, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Sender> senders, CancellationToken cancellationToken)
  {
    await base.SaveAsync(senders, cancellationToken);
  }
}
