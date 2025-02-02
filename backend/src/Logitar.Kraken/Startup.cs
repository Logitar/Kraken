using Logitar.Kraken.Constants;
using Logitar.Kraken.Core;
using Logitar.Kraken.Infrastructure;
using Logitar.Kraken.Web;
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
    //application.UseCors(application.Services.GetRequiredService<CorsSettings>()); // TODO(fpion): CORS
    application.UseStaticFiles();
    //application.UseExceptionHandler(); // TODO(fpion): ExceptionHandler
    //application.UseSession(); // TODO(fpion): Session
    //application.UseMiddleware<RenewSession>(); // TODO(fpion): Session
    //application.UseMiddleware<RedirectNotFound>(); // TODO(fpion): Frontend
    //application.UseAuthentication(); // TODO(fpion): Authentication
    //application.UseAuthorization(); // TODO(fpion): Authorization

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
