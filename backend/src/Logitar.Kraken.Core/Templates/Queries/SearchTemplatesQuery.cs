using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Templates;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Queries;

public record SearchTemplatesQuery(SearchTemplatesPayload Payload) : Activity, IRequest<SearchResults<TemplateModel>>;

internal class SearchTemplatesQueryHandler : IRequestHandler<SearchTemplatesQuery, SearchResults<TemplateModel>>
{
  private readonly ITemplateQuerier _templateQuerier;

  public SearchTemplatesQueryHandler(ITemplateQuerier templateQuerier)
  {
    _templateQuerier = templateQuerier;
  }

  public async Task<SearchResults<TemplateModel>> Handle(SearchTemplatesQuery query, CancellationToken cancellationToken)
  {
    return await _templateQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
