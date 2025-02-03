namespace Logitar.Kraken.Core.Contents;

public interface IContentManager
{
  Task SaveAsync(Content content, CancellationToken cancellationToken = default);
  Task SaveAsync(Content content, ContentType contentType, CancellationToken cancellationToken = default);
}
