using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Contents;

public interface IContentTypeRepository
{
  Task<ContentType?> LoadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<ContentType?> LoadAsync(ContentTypeId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<ContentType?> LoadAsync(ContentTypeId id, long? version, CancellationToken cancellationToken = default);
  Task<ContentType?> LoadAsync(ContentTypeId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<ContentType>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentType>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<ContentType>> LoadAsync(IEnumerable<ContentTypeId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentType>> LoadAsync(IEnumerable<ContentTypeId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<ContentType?> LoadAsync(RealmId? realmId, Identifier uniqueName, CancellationToken cancellationToken = default);
  Task<ContentType> LoadAsync(Content content, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentType>> LoadAsync(FieldTypeId fieldTypeId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentType>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);

  Task SaveAsync(ContentType contentType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ContentType> contentTypes, CancellationToken cancellationToken = default);
}
