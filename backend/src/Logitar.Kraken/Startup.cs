using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Kraken.Constants;
using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational;
using Logitar.Kraken.EntityFrameworkCore.SqlServer;
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
    services.AddLogitarKrakenEntityFrameworkCoreRelational();
    services.AddLogitarKrakenWeb(_configuration);

    services.AddOpenApi();

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services.AddFeatureManagement();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarKrakenEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<KrakenContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
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
    application.UseExceptionHandler();
    //application.UseSession(); // ISSUE #34: https://github.com/Logitar/Kraken/issues/34
    //application.UseMiddleware<RenewSession>(); // ISSUE #34: https://github.com/Logitar/Kraken/issues/34
    //application.UseMiddleware<RedirectNotFound>(); // ISSUE #37: https://github.com/Logitar/Kraken/issues/37
    //application.UseAuthentication(); // ISSUE #35: https://github.com/Logitar/Kraken/issues/35
    //application.UseAuthorization(); // ISSUE #36: https://github.com/Logitar/Kraken/issues/36

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
