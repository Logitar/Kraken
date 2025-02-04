using Logitar.Kraken.Contracts.Templates;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Queries;

public record ReadTemplateQuery(Guid? Id, string? UniqueKey) : Activity, IRequest<TemplateModel?>;

internal class ReadTemplateQueryHandler : IRequestHandler<ReadTemplateQuery, TemplateModel?>
{
  private readonly ITemplateQuerier _templateQuerier;

  public ReadTemplateQueryHandler(ITemplateQuerier templateQuerier)
  {
    _templateQuerier = templateQuerier;
  }

  public async Task<TemplateModel?> Handle(ReadTemplateQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, TemplateModel> templates = new(capacity: 2);

    if (query.Id.HasValue)
    {
      TemplateModel? template = await _templateQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueKey))
    {
      TemplateModel? template = await _templateQuerier.ReadAsync(query.UniqueKey, cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (templates.Count > 1)
    {
      throw TooManyResultsException<TemplateModel>.ExpectedSingle(templates.Count);
    }

    return templates.Values.SingleOrDefault();
  }
}
