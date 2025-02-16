using Logitar.Kraken.Core;
using Logitar.Kraken.Web.Settings;

namespace Logitar.Kraken.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddControllersWithViews()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    services.AddSingleton(CookiesSettings.Initialize(configuration));
    services.AddSingleton(CorsSettings.Initialize(configuration));
    services.AddSingleton(OpenAuthenticationSettings.Initialize(configuration));

    return services.AddSingleton<IApplicationContext, HttpApplicationContext>();
  }
}
