using Logitar.Kraken.Contracts.Contents;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Queries;

public record ReadPublishedContentQuery(int? ContentId, Guid? ContentUid, PublishedContentKey? Key) : IRequest<PublishedContent?>;

internal class ReadPublishedContentQueryHandler : IRequestHandler<ReadPublishedContentQuery, PublishedContent?>
{
  private readonly IPublishedContentQuerier _publishedContentQuerier;

  public ReadPublishedContentQueryHandler(IPublishedContentQuerier publishedContentQuerier)
  {
    _publishedContentQuerier = publishedContentQuerier;
  }

  public async Task<PublishedContent?> Handle(ReadPublishedContentQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, PublishedContent> publishedContents = new(capacity: 3);

    if (query.ContentId.HasValue)
    {
      PublishedContent? publishedContent = await _publishedContentQuerier.ReadAsync(query.ContentId.Value, cancellationToken);
      if (publishedContent != null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (query.ContentUid.HasValue)
    {
      PublishedContent? publishedContent = await _publishedContentQuerier.ReadAsync(query.ContentUid.Value, cancellationToken);
      if (publishedContent != null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (query.Key != null)
    {
      PublishedContent? publishedContent = await _publishedContentQuerier.ReadAsync(query.Key, cancellationToken);
      if (publishedContent != null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (publishedContents.Count > 1)
    {
      throw TooManyResultsException<PublishedContent>.ExpectedSingle(publishedContents.Count);
    }

    return publishedContents.SingleOrDefault().Value;
  }
}
