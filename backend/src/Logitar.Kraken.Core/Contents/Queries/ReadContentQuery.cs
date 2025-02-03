using Logitar.Kraken.Contracts.Contents;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Queries;

/// <exception cref="TooManyResultsException"></exception>
public record ReadContentQuery(Guid? Id, ContentKey? Key) : IRequest<ContentModel?>;

internal class ReadContentQueryHandler : IRequestHandler<ReadContentQuery, ContentModel?>
{
  private readonly IContentQuerier _contentQuerier;

  public ReadContentQueryHandler(IContentQuerier contentQuerier)
  {
    _contentQuerier = contentQuerier;
  }

  public async Task<ContentModel?> Handle(ReadContentQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ContentModel> contents = new(capacity: 2);

    if (query.Id.HasValue)
    {
      ContentModel? content = await _contentQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (content != null)
      {
        contents[content.Id] = content;
      }
    }

    if (query.Key != null)
    {
      ContentModel? content = await _contentQuerier.ReadAsync(query.Key, cancellationToken);
      if (content != null)
      {
        contents[content.Id] = content;
      }
    }

    if (contents.Count > 1)
    {
      throw TooManyResultsException<ContentModel>.ExpectedSingle(contents.Count);
    }

    return contents.SingleOrDefault().Value;
  }
}
