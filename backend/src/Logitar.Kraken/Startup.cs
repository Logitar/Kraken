using Logitar.Kraken.Constants;
using Logitar.Kraken.Core;
using Logitar.Kraken.Infrastructure;
using Logitar.Kraken.Web;
using Logitar.Kraken.Web.Extensions;
using Logitar.Kraken.Web.Settings;
using Microsoft.FeatureManagement;
using Scalar.AspNetCore;

namespace Logitar.Kraken;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddLogitarKrakenCore();
    services.AddLogitarKrakenInfrastructure();
    services.AddLogitarKrakenWeb(_configuration);

    services.AddOpenApi();

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services.AddFeatureManagement();
    services.AddProblemDetails();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    throw new NotSupportedException();
  }
  public async Task ConfigureAsync(WebApplication application)
  {
    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();

    if (await featureManager.IsEnabledAsync(Features.UseScalarUI))
    {
      application.MapOpenApi();
      application.MapScalarApiReference();
    }

    application.UseHttpsRedirection();
    application.UseCors(application.Services.GetRequiredService<CorsSettings>());
    application.UseStaticFiles();
    //application.UseExceptionHandler(); // ISSUE #33: https://github.com/Logitar/Kraken/issues/33
    //application.UseSession(); // ISSUE #34: https://github.com/Logitar/Kraken/issues/34
    //application.UseMiddleware<RenewSession>(); // ISSUE #34: https://github.com/Logitar/Kraken/issues/34
    //application.UseMiddleware<RedirectNotFound>(); // ISSUE #37: https://github.com/Logitar/Kraken/issues/37
    //application.UseAuthentication(); // ISSUE #35: https://github.com/Logitar/Kraken/issues/35
    //application.UseAuthorization(); // ISSUE #36: https://github.com/Logitar/Kraken/issues/36

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
