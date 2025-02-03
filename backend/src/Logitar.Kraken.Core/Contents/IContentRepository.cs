namespace Logitar.Kraken.Core.Contents;

public interface IContentRepository
{
  Task<Content?> LoadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<Content?> LoadAsync(ContentId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Content contentType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Content> contentTypes, CancellationToken cancellationToken = default);
}
