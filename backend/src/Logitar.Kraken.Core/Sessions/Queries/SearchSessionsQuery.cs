using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Sessions;
using MediatR;

namespace Logitar.Kraken.Core.Sessions.Queries;

public record SearchSessionsQuery(SearchSessionsPayload Payload) : Activity, IRequest<SearchResults<SessionModel>>;

internal class SearchSessionsQueryHandler : IRequestHandler<SearchSessionsQuery, SearchResults<SessionModel>>
{
  private readonly ISessionQuerier _sessionQuerier;

  public SearchSessionsQueryHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<SearchResults<SessionModel>> Handle(SearchSessionsQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
