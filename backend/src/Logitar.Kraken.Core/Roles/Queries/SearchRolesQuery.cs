using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Search;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Queries;

public record SearchRolesQuery(SearchRolesPayload Payload) : Activity, IRequest<SearchResults<RoleModel>>;

internal class SearchRolesQueryHandler : IRequestHandler<SearchRolesQuery, SearchResults<RoleModel>>
{
  private readonly IRoleQuerier _roleQuerier;

  public SearchRolesQueryHandler(IRoleQuerier roleQuerier)
  {
    _roleQuerier = roleQuerier;
  }

  public async Task<SearchResults<RoleModel>> Handle(SearchRolesQuery query, CancellationToken cancellationToken)
  {
    return await _roleQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
