using Logitar.Kraken.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Queries;

public record ReadApiKeyQuery(Guid Id) : Activity, IRequest<ApiKeyModel?>;

internal class ReadApiKeyQueryHandler : IRequestHandler<ReadApiKeyQuery, ApiKeyModel?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;

  public ReadApiKeyQueryHandler(IApiKeyQuerier apiKeyQuerier)
  {
    _apiKeyQuerier = apiKeyQuerier;
  }

  public async Task<ApiKeyModel?> Handle(ReadApiKeyQuery query, CancellationToken cancellationToken)
  {
    return await _apiKeyQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
