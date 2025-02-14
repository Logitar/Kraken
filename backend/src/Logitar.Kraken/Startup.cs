using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Kraken.Constants;
using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.SqlServer;
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
    services.AddLogitarEventSourcingWithEntityFrameworkCoreRelational();
    services.AddLogitarKrakenWeb(_configuration);

    services.AddOpenApi();

    services.AddFeatureManagement();

    DatabaseProvider databaseProvider = GetDatabaseProvider();
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarKrakenEntityFrameworkCoreSqlServer(_configuration);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }
  private DatabaseProvider GetDatabaseProvider()
  {
    string? value = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");
    if (!string.IsNullOrWhiteSpace(value))
    {
      return Enum.Parse<DatabaseProvider>(value.Trim());
    }
    return _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      ConfigureAsync(application).Wait();
    }
  }
  public async Task ConfigureAsync(WebApplication application)
  {
    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();
    if (await featureManager.IsEnabledAsync(Features.UseScalarUi))
    {
      application.MapOpenApi();
      application.MapScalarApiReference();
    }

    application.UseHttpsRedirection();

    application.MapControllers();
  }
}
