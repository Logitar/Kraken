using Logitar.EventSourcing;
using Logitar.Kraken.Core.Messages;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class MessageRepository : Repository, IMessageRepository
{
  private readonly DbSet<MessageEntity> _messages;

  public MessageRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _messages = context.Messages;
  }

  public async Task<Message?> LoadAsync(MessageId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Message?> LoadAsync(MessageId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Message?> LoadAsync(MessageId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Message?> LoadAsync(MessageId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Message>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Message>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Message>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Message>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Message>> LoadAsync(IEnumerable<MessageId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Message>> LoadAsync(IEnumerable<MessageId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Message>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Message>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _messages.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Message>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task SaveAsync(Message message, CancellationToken cancellationToken)
  {
    await base.SaveAsync(message, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(messages, cancellationToken);
  }
}
