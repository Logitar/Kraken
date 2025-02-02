using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Caching;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Queries;

public record ReadConfigurationQuery : IRequest<ConfigurationModel>;

internal class ReadConfigurationQueryHandler : IRequestHandler<ReadConfigurationQuery, ConfigurationModel>
{
  private readonly ICacheService _cacheService;

  public ReadConfigurationQueryHandler(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public Task<ConfigurationModel> Handle(ReadConfigurationQuery _, CancellationToken cancellationToken)
  {
    ConfigurationModel configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration should be in the cache.");
    return Task.FromResult(configuration);
  }
}
