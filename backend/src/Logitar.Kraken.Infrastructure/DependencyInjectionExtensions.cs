using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenInfrastructure(this IServiceCollection services)
  {
    return services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }
}
