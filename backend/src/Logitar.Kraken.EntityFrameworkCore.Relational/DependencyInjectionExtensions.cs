using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Configurations;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Actors;
using Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddScoped<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationQuerier, ConfigurationQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>();
  }
}
