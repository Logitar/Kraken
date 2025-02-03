namespace Logitar.Kraken.Core.Contents;

public interface IContentTypeManager
{
  Task SaveAsync(ContentType contentType, CancellationToken cancellationToken = default);
}
