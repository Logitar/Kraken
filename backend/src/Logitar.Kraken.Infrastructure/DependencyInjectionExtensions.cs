using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Logitar.Kraken.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .RemoveAll<IEventSerializer>()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddMemoryCache()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddScoped<IEventBus, EventBus>()
      .AddQueriers()
      .AddRepositories();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services;
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services;
  }
}
