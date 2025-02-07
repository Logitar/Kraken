using Logitar.EventSourcing.Infrastructure;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Infrastructure.Caching;
using Logitar.Kraken.Infrastructure.Converters;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenInfrastructure(this IServiceCollection services)
  {
    return services
      .AddMemoryCache()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddSingleton<PasswordConverter>()
      .AddScoped<IEventBus, EventBus>();
  }
}
