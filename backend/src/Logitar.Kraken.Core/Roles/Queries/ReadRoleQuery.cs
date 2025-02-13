using Logitar.Kraken.Contracts.Roles;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Queries;

public record ReadRoleQuery(Guid? Id, string? UniqueName) : Activity, IRequest<RoleModel?>;

internal class ReadRoleQueryHandler : IRequestHandler<ReadRoleQuery, RoleModel?>
{
  private readonly IRoleQuerier _roleQuerier;

  public ReadRoleQueryHandler(IRoleQuerier roleQuerier)
  {
    _roleQuerier = roleQuerier;
  }

  public async Task<RoleModel?> Handle(ReadRoleQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, RoleModel> roles = new(capacity: 2);

    if (query.Id.HasValue)
    {
      RoleModel? role = await _roleQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      RoleModel? role = await _roleQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw TooManyResultsException<RoleModel>.ExpectedSingle(roles.Count);
    }

    return roles.SingleOrDefault().Value;
  }
}
