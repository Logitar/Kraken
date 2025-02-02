using Logitar.Kraken.Core;

namespace Logitar.Kraken.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddControllersWithViews(/*options => options.Filters.Add<LoggingFilter>()*/) // TODO(fpion): Logging
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    //CookiesSettings cookiesSettings = configuration.GetSection(CookiesSettings.SectionKey).Get<CookiesSettings>() ?? new();
    //services.AddSingleton(cookiesSettings); // TODO(fpion): Session

    //CorsSettings corsSettings = configuration.GetSection(CorsSettings.SectionKey).Get<CorsSettings>() ?? new();
    //services.AddSingleton(corsSettings); // TODO(fpion): CORS

    //OpenAuthenticationSettings openAuthenticationSettings = configuration.GetSection(OpenAuthenticationSettings.SectionKey).Get<OpenAuthenticationSettings>() ?? new();
    //services.AddSingleton(openAuthenticationSettings); // TODO(fpion): Authentication

    return services
      .AddSingleton<IApplicationContext, HttpApplicationContext>()
      /*.AddTransient<IOpenAuthenticationService, OpenAuthenticationService>()*/; // TODO(fpion): Authentication
  }
}
