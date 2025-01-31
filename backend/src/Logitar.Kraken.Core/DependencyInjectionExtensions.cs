using FluentValidation.Resources;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenCore(this IServiceCollection services)
  {
    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddManagers();
  }

  private static IServiceCollection AddManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<ILanguageManager, LanguageManager>()
      .AddTransient<IRealmManager, RealmManager>()
      .AddTransient<IRoleManager, RoleManager>();
  }
}
