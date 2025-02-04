using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Contents;

public interface IContentRepository
{
  Task<Content?> LoadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<Content?> LoadAsync(ContentId id, long? version, CancellationToken cancellationToken = default);
  Task<Content?> LoadAsync(ContentId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<Content?> LoadAsync(ContentId id, bool? isDeleted, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Content>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Content>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Content>> LoadAsync(IEnumerable<ContentId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Content>> LoadAsync(bool? isDeleted, IEnumerable<ContentId> ids, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Content>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);

  Task SaveAsync(Content contentType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Content> contentTypes, CancellationToken cancellationToken = default);
}
