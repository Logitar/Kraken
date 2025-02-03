using Logitar.Kraken.Contracts.Configurations;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Queries;

public record ReadConfigurationQuery : IRequest<ConfigurationModel>;

internal class ReadConfigurationQueryHandler : IRequestHandler<ReadConfigurationQuery, ConfigurationModel>
{
  private readonly IApplicationContext _applicationContext;

  public ReadConfigurationQueryHandler(IApplicationContext applicationContext)
  {
    _applicationContext = applicationContext;
  }

  public Task<ConfigurationModel> Handle(ReadConfigurationQuery _, CancellationToken cancellationToken)
  {
    ConfigurationModel configuration = _applicationContext.Configuration;
    return Task.FromResult(configuration);
  }
}
