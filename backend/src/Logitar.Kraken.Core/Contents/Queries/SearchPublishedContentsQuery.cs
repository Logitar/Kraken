using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Queries;

public record SearchPublishedContentsQuery(SearchPublishedContentsPayload Payload) : IRequest<SearchResults<PublishedContentLocale>>;

internal class SearchPublishedContentsQueryHandler : IRequestHandler<SearchPublishedContentsQuery, SearchResults<PublishedContentLocale>>
{
  private readonly IPublishedContentQuerier _publishedContentQuerier;

  public SearchPublishedContentsQueryHandler(IPublishedContentQuerier publishedContentQuerier)
  {
    _publishedContentQuerier = publishedContentQuerier;
  }

  public async Task<SearchResults<PublishedContentLocale>> Handle(SearchPublishedContentsQuery query, CancellationToken cancellationToken)
  {
    return await _publishedContentQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
