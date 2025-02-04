using Logitar.Kraken.Core;
using Logitar.Kraken.Web.Settings;

namespace Logitar.Kraken.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddControllersWithViews(/*options => options.Filters.Add<LoggingFilter>()*/) // ISSUE #41: https://github.com/Logitar/Kraken/issues/41
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    //CookiesSettings cookiesSettings = configuration.GetSection(CookiesSettings.SectionKey).Get<CookiesSettings>() ?? new();
    //services.AddSingleton(cookiesSettings); // ISSUE #34: https://github.com/Logitar/Kraken/issues/34

    CorsSettings corsSettings = configuration.GetSection(CorsSettings.SectionKey).Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors();

    //OpenAuthenticationSettings openAuthenticationSettings = configuration.GetSection(OpenAuthenticationSettings.SectionKey).Get<OpenAuthenticationSettings>() ?? new();
    //services.AddSingleton(openAuthenticationSettings); // ISSUE #35: https://github.com/Logitar/Kraken/issues/35

    return services
      .AddSingleton<IApplicationContext, HttpApplicationContext>()
      /*.AddTransient<IOpenAuthenticationService, OpenAuthenticationService>()*/; // ISSUE #35: https://github.com/Logitar/Kraken/issues/35
  }
}
