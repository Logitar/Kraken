using Logitar.Kraken.Core;

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

    return services.AddSingleton<IApplicationContext, HttpApplicationContext>();
  }
}
