using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Core.Roles;

public interface IRoleQuerier
{
  Task<RoleId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<RoleId>> FindIdsAsync(IEnumerable<string> uniqueNames, CancellationToken cancellationToken = default);

  Task<RoleModel> ReadAsync(Role role, CancellationToken cancellationToken = default);
  Task<RoleModel?> ReadAsync(RoleId id, CancellationToken cancellationToken = default);
  Task<RoleModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RoleModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<RoleModel>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken = default);
}
