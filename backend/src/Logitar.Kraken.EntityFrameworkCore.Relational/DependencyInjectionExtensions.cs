using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
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
