using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class PublishedContentQuerier : IPublishedContentQuerier
{
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<PublishedContentEntity> _publishedContents;

  public PublishedContentQuerier(IApplicationContext applicationContext, KrakenContext context)
  {
    _applicationContext = applicationContext;
    _publishedContents = context.PublishedContents;
  }

  public Task<PublishedContent?> ReadAsync(int contentId, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(Guid contentId, CancellationToken cancellationToken)
  {
  }

  public Task<PublishedContent?> ReadAsync(PublishedContentKey key, CancellationToken cancellationToken)
  {
  }

  public Task<PublishedContent?> ReadAsync(int contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(Guid contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(string contentTypeName, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(int contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(Guid contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(string contentTypeName, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(int contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(Guid contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
  }
  public Task<PublishedContent?> ReadAsync(string contentTypeName, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
  }

  public Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken)
  {
  }
}
