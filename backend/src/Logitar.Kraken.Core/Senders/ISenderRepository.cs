using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Senders;

public interface ISenderRepository
{
  Task<Sender?> LoadAsync(SenderId id, CancellationToken cancellationToken = default);
  Task<Sender?> LoadAsync(SenderId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<Sender?> LoadAsync(SenderId id, long? version, CancellationToken cancellationToken = default);
  Task<Sender?> LoadAsync(SenderId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Sender>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Sender>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Sender>> LoadAsync(IEnumerable<SenderId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Sender>> LoadAsync(IEnumerable<SenderId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Sender>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);
  Task<Sender?> LoadDefaultAsync(RealmId? realmId, SenderType type, CancellationToken cancellationToken = default);

  Task SaveAsync(Sender sender, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Sender> senders, CancellationToken cancellationToken = default);
}
