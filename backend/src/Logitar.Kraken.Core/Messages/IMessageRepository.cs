using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Messages;

public interface IMessageRepository
{
  Task<Message?> LoadAsync(MessageId id, CancellationToken cancellationToken = default);
  Task<Message?> LoadAsync(MessageId id, long? version, CancellationToken cancellationToken = default);
  Task<Message?> LoadAsync(MessageId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<Message?> LoadAsync(MessageId id, bool? isDeleted, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Message>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Message>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Message>> LoadAsync(IEnumerable<MessageId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Message>> LoadAsync(IEnumerable<MessageId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Message>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);

  Task SaveAsync(Message message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default);
}
