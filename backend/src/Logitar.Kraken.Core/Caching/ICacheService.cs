using Logitar.Kraken.Contracts.Configurations;

namespace Logitar.Kraken.Core.Caching;

public interface ICacheService
{
  ConfigurationModel? Configuration { get; set; }
}
