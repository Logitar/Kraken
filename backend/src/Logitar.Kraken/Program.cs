using Logitar.Kraken.Constants;
using Logitar.Kraken.Core.Configurations.Commands;
using Logitar.Kraken.EntityFrameworkCore.Relational.Commands;
using Logitar.Kraken.Settings;
using MediatR;
using Microsoft.FeatureManagement;

namespace Logitar.Kraken;

internal class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    await startup.ConfigureAsync(application);

    using IServiceScope scope = application.Services.CreateScope();
    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    IFeatureManager featureManager = scope.ServiceProvider.GetRequiredService<IFeatureManager>();
    if (await featureManager.IsEnabledAsync(Features.MigrateDatabase))
    {
      await mediator.Send(new MigrateDatabaseCommand());
    }

    DefaultSettings defaults = DefaultSettings.Initialize(configuration);
    await mediator.Send(new InitializeConfigurationCommand(defaults.Locale, defaults.UniqueName, defaults.Password));

    application.Run();
  }
}
