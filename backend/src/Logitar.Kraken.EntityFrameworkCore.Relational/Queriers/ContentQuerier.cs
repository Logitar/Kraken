using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class ContentQuerier : IContentQuerier
{
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<ContentEntity> _contents;

  public ContentQuerier(IApplicationContext applicationContext, KrakenContext context)
  {
    _applicationContext = applicationContext;
    _contents = context.Contents;
  }

  public Task<IReadOnlyDictionary<Guid, ContentId>> FindConflictsAsync(ContentTypeId contentTypeId, LanguageId? languageId, IReadOnlyDictionary<Guid, string> fieldValues, ContentId contentId, CancellationToken cancellationToken)
  {
  }

  public Task<IReadOnlyDictionary<Guid, Guid>> FindContentTypeIdsAsync(IEnumerable<Guid> contentIds, CancellationToken cancellationToken)
  {
  }

  public Task<ContentId?> FindIdAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueName uniqueName, CancellationToken cancellationToken)
  {
  }

  public async Task<ContentModel> ReadAsync(Content content, CancellationToken cancellationToken)
  {
    return await ReadAsync(content.Id, cancellationToken) ?? throw new InvalidOperationException($"The content entity 'StreamId={content.Id}' could not be found.");
  }
  public async Task<ContentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new ContentId(_applicationContext.RealmId, id), cancellationToken);
  }
  public Task<ContentModel?> ReadAsync(ContentId id, CancellationToken cancellationToken)
  {
  }
  public Task<ContentModel?> ReadAsync(ContentKey key, CancellationToken cancellationToken)
  {
  }

  public Task<SearchResults<ContentLocaleModel>> SearchAsync(SearchContentsPayload payload, CancellationToken cancellationToken)
  {
  }
}
