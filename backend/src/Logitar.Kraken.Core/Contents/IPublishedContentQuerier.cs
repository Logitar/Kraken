using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Core.Contents;

public interface IPublishedContentQuerier
{
  Task<PublishedContent?> ReadAsync(int contentId, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(Guid contentId, CancellationToken cancellationToken = default);

  Task<PublishedContent?> ReadAsync(PublishedContentKey key, CancellationToken cancellationToken = default);

  Task<PublishedContent?> ReadAsync(int contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(Guid contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(string contentTypeName, int? languageId, string uniqueName, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(int contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(Guid contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(string contentTypeName, Guid? languageId, string uniqueName, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(int contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(Guid contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(string contentTypeName, string? languageCode, string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken = default);
}
