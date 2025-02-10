using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class ContentTypeQuerier : IContentTypeQuerier
{
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<ContentTypeEntity> _contentTypes;

  public ContentTypeQuerier(IApplicationContext applicationContext, KrakenContext context)
  {
    _applicationContext = applicationContext;
    _contentTypes = context.ContentTypes;
  }

  public Task<ContentTypeId?> FindIdAsync(Identifier uniqueName, CancellationToken cancellationToken)
  {
  }

  public async Task<ContentTypeModel> ReadAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    return await ReadAsync(contentType.Id, cancellationToken) ?? throw new InvalidOperationException($"The content type entity 'StreamId={contentType.Id}' could not be found.");
  }
  public async Task<ContentTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new ContentTypeId(_applicationContext.RealmId, id), cancellationToken);
  }
  public Task<ContentTypeModel?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
  }
  public Task<ContentTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
  }

  public Task<SearchResults<ContentTypeModel>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken)
  {
  }
}
