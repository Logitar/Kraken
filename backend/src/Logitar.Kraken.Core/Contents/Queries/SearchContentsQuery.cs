using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Queries;

public record SearchContentsQuery(SearchContentsPayload Payload) : IRequest<SearchResults<ContentLocaleModel>>;

internal class SearchContentsQueryHandler : IRequestHandler<SearchContentsQuery, SearchResults<ContentLocaleModel>>
{
  private readonly IContentQuerier _contentQuerier;

  public SearchContentsQueryHandler(IContentQuerier contentQuerier)
  {
    _contentQuerier = contentQuerier;
  }

  public async Task<SearchResults<ContentLocaleModel>> Handle(SearchContentsQuery query, CancellationToken cancellationToken)
  {
    return await _contentQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
