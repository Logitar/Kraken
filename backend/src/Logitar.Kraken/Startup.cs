using Logitar.Kraken.Constants;
using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational;
using Logitar.Kraken.EntityFrameworkCore.SqlServer;
using Logitar.Kraken.Infrastructure;
using Logitar.Kraken.Web;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Extensions;
using Logitar.Kraken.Web.Settings;
using Microsoft.FeatureManagement;
using Scalar.AspNetCore;

namespace Logitar.Kraken;

internal class Startup : StartupBase
{
  private readonly string[] _authenticationSchemes;
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _authenticationSchemes = Schemes.GetEnabled(configuration);
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddLogitarKrakenCore();
    services.AddLogitarKrakenInfrastructure();
    services.AddLogitarKrakenEntityFrameworkCoreRelational();
    services.AddLogitarKrakenWeb(_configuration);

    services.AddCors();

    //AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
    //  .AddScheme<ApiAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
    //  .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
    //  .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    //if (_authenticationSchemes.Contains(Schemes.Basic))
    //{
    //  authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    //} // TODO(fpion): Authentication

    //services.AddAuthorizationBuilder()
    //  .SetDefaultPolicy(new AuthorizationPolicyBuilder(_authenticationSchemes).RequireAuthenticatedUser().Build()); // TODO(fpion): Authorization

    //CookiesSettings cookiesSettings = _configuration.GetSection(CookiesSettings.SectionKey).Get<CookiesSettings>() ?? new();
    //services.AddSession(options =>
    //{
    //  options.Cookie.SameSite = cookiesSettings.Session.SameSite;
    //  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //}); // TODO(fpion): Session

    services.AddApplicationInsightsTelemetry();
    //IHealthChecksBuilder healthChecks = services.AddHealthChecks(); // TODO(fpion): HealthChecks

    services.AddOpenApi();

    //services.AddDistributedMemoryCache(); // TODO(fpion): Session
    //services.AddExceptionHandler<ExceptionHandler>(); // TODO(fpion): ExceptionHandler
    services.AddFeatureManagement();
    //services.AddProblemDetails(); // TODO(fpion): ExceptionHandler

    DatabaseProvider databaseProvider = EnvironmentHelper.GetEnum("DATABASE_PROVIDER", _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer);
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarKrakenEntityFrameworkCoreSqlServer(_configuration);
        //healthChecks.AddDbContextCheck<EventContext>(); // TODO(fpion): HealthChecks
        //healthChecks.AddDbContextCheck<KrakenContext>(); // TODO(fpion): HealthChecks
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
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
    application.UseCors(application.Services.GetRequiredService<CorsSettings>());
    //application.UseStaticFiles(); // TODO(fpion): Frontend Integration
    //application.UseExceptionHandler(); // TODO(fpion): ExceptionHandler
    //application.UseSession(); // TODO(fpion): Session
    //application.UseMiddleware<RenewSession>(); // TODO(fpion): Session
    //application.UseMiddleware<RedirectNotFound>(); // TODO(fpion): Frontend Integration
    //application.UseAuthentication(); // TODO(fpion): Authentication
    //application.UseAuthorization(); // TODO(fpion): Authorization

    application.MapControllers();
    //application.MapHealthChecks("/health"); // TODO(fpion): HealthChecks
  }
}
