using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Templates;

public interface ITemplateRepository
{
  Task<Template?> LoadAsync(TemplateId id, CancellationToken cancellationToken = default);
  Task<Template?> LoadAsync(TemplateId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<Template?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken = default);
  Task<Template?> LoadAsync(TemplateId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Template>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Template>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Template>> LoadAsync(IEnumerable<TemplateId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Template>> LoadAsync(IEnumerable<TemplateId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Template>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);
  Task<Template?> LoadAsync(RealmId? realmId, Identifier uniqueKey, CancellationToken cancellationToken = default);

  Task SaveAsync(Template template, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Template> templates, CancellationToken cancellationToken = default);
}
