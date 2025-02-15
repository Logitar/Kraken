using Logitar.Kraken.Core;
using Logitar.Kraken.Web.Settings;

namespace Logitar.Kraken.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    services.AddSingleton(CookiesSettings.Initialize(configuration));
    services.AddSingleton(CorsSettings.Initialize(configuration));
    // TODO(fpion): OAuth settings

    return services.AddSingleton<IApplicationContext, HttpApplicationContext>();
  }
}
