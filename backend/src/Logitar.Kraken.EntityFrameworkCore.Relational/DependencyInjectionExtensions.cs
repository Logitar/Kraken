using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Configurations;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Actors;
using Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;
using Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddRepositories()
      .AddScoped<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationQuerier, ConfigurationQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationRepository, ConfigurationRepository>()
      .AddScoped<IRealmRepository, RealmRepository>();
  }
}
