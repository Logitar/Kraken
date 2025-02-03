using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Contracts.Search;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Queries;

public record SearchFieldTypesQuery(SearchFieldTypesPayload Payload) : IRequest<SearchResults<FieldTypeModel>>;

internal class SearchFieldTypesQueryHandler : IRequestHandler<SearchFieldTypesQuery, SearchResults<FieldTypeModel>>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;

  public SearchFieldTypesQueryHandler(IFieldTypeQuerier fieldTypeQuerier)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
  }

  public async Task<SearchResults<FieldTypeModel>> Handle(SearchFieldTypesQuery query, CancellationToken cancellationToken)
  {
    return await _fieldTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
