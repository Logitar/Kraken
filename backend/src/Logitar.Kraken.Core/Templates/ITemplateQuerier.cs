using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Templates;

namespace Logitar.Kraken.Core.Templates;

public interface ITemplateQuerier
{
  Task<TemplateId?> FindIdAsync(Identifier uniqueKey, CancellationToken cancellationToken = default);

  Task<TemplateModel> ReadAsync(Template template, CancellationToken cancellationToken = default);
  Task<TemplateModel?> ReadAsync(TemplateId id, CancellationToken cancellationToken = default);
  Task<TemplateModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TemplateModel?> ReadAsync(string uniqueKey, CancellationToken cancellationToken = default);

  Task<SearchResults<TemplateModel>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken = default);
}
