using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Sessions;

namespace Logitar.Kraken.Core.Users;

public interface IUserRepository
{
  Task<User?> LoadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<User?> LoadAsync(UserId id, long? version, CancellationToken cancellationToken = default);
  Task<User?> LoadAsync(UserId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<User?> LoadAsync(UserId id, bool? isDeleted, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<User>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<User>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<User> LoadAsync(Session session, CancellationToken cancellationToken = default);
  Task<User?> LoadAsync(RealmId? realmId, UniqueName uniqueName, CancellationToken cancellationToken = default);
  Task<User?> LoadAsync(RealmId? realmId, Identifier identifierKey, CustomIdentifier identifierValue, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<User>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<User>> LoadAsync(RealmId? realmId, IEmail email, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<User>> LoadAsync(RoleId roleId, CancellationToken cancellationToken = default);

  Task SaveAsync(User user, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
}
