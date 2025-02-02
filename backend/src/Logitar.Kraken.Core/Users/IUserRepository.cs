using Logitar.Kraken.Core.Sessions;

namespace Logitar.Kraken.Core.Users;

public interface IUserRepository
{
  Task<User> LoadAsync(Session session, CancellationToken cancellationToken = default);
  Task<User?> LoadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<User?> LoadAsync(UserId id, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<User>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(User user, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
}
