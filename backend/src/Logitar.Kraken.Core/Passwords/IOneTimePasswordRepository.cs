using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Passwords;

public interface IOneTimePasswordRepository
{
  Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, long? version, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, bool? isDeleted, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(IEnumerable<OneTimePasswordId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(IEnumerable<OneTimePasswordId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);

  Task SaveAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<OneTimePassword> oneTimePasswords, CancellationToken cancellationToken = default);
}
