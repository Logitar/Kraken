using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.Core.Configurations;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;
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
    return services
      .AddScoped<IApiKeyRepository, ApiKeyRepository>()
      .AddScoped<IConfigurationRepository, ConfigurationRepository>()
      .AddScoped<IRoleRepository, RoleRepository>()
      .AddScoped<IUserRepository, UserRepository>();
  }
}
