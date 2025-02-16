using Logitar.EventSourcing;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenCore(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcing()
      .AddManagers()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton<IAddressHelper, AddressHelper>();
  }

  private static IServiceCollection AddManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<IDictionaryManager, DictionaryManager>()
      .AddTransient<ILanguageManager, LanguageManager>()
      .AddTransient<IRealmManager, RealmManager>()
      .AddTransient<IRoleManager, RoleManager>()
      .AddTransient<IUserManager, UserManager>();
  }
}
