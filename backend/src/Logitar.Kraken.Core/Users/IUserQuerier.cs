using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core.Users;

public interface IUserQuerier
{
  Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<UserModel> ReadAsync(User user, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<UserModel>> ReadAsync(IEmail email, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(CustomIdentifierModel customIdentifier, CancellationToken cancellationToken = default);

  Task<SearchResults<UserModel>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken = default);
}
