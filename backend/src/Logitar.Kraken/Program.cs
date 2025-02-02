using Logitar.Kraken.Constants;
using Logitar.Kraken.Core.Configurations.Commands;
using Logitar.Kraken.Infrastructure.Commands;
using MediatR;
using Microsoft.FeatureManagement;

namespace Logitar.Kraken;

public class Program
{
  private const string DefaultUniqueName = "admin";
  private const string DefaultPassword = "P@s$W0rD";
  private const string DefaultLocale = "en";

  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    await startup.ConfigureAsync(application);

    IServiceScope scope = application.Services.CreateScope();
    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();
    if (await featureManager.IsEnabledAsync(Features.MigrateDatabase))
    {
      await mediator.Send(new MigrateDatabaseCommand());
    }

    string uniqueName = configuration.GetValue<string>("KRAKEN_USERNAME") ?? DefaultUniqueName;
    string password = configuration.GetValue<string>("KRAKEN_PASSWORD") ?? DefaultPassword;
    string defaultLocale = configuration.GetValue<string>("KRAKEN_LOCALE") ?? DefaultLocale;
    await mediator.Send(new InitializeConfigurationCommand(uniqueName, password, defaultLocale));

    application.Run();
  }
}
