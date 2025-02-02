using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Users;
using MediatR;

namespace Logitar.Kraken.Core.Users.Queries;

public record SearchUsersQuery(SearchUsersPayload Payload) : Activity, IRequest<SearchResults<UserModel>>;

internal class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, SearchResults<UserModel>>
{
  private readonly IUserQuerier _userQuerier;

  public SearchUsersQueryHandler(IUserQuerier sessionQuerier)
  {
    _userQuerier = sessionQuerier;
  }

  public async Task<SearchResults<UserModel>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
  {
    return await _userQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
