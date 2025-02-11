using Logitar.Kraken.Core.Configurations.Commands;
using Logitar.Kraken.Settings;
using MediatR;

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

    IMediator mediator = application.Services.GetRequiredService<IMediator>();
    DefaultSettings defaults = DefaultSettings.Initialize(configuration);
    await mediator.Send(new InitializeConfigurationCommand(defaults.Locale, defaults.UniqueName, defaults.Password));

    application.Run();
  }
}
