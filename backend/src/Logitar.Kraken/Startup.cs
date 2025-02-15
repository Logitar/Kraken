using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Kraken.Constants;
using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational;
using Logitar.Kraken.EntityFrameworkCore.SqlServer;
using Logitar.Kraken.Infrastructure;
using Logitar.Kraken.Web;
using Logitar.Kraken.Web.Authentication;
using Logitar.Kraken.Web.Authorization;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Extensions;
using Logitar.Kraken.Web.Middlewares;
using Logitar.Kraken.Web.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (_authenticationSchemes.Contains(Schemes.Basic))
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }
    // TODO(fpion): OpenAuthenticationService implementation

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder(_authenticationSchemes).RequireAuthenticatedUser().Build())
      .AddPolicy(Policies.KrakenAdmin, new AuthorizationPolicyBuilder(_authenticationSchemes)
        .RequireAuthenticatedUser()
        .AddRequirements(new KrakenAdminRequirement())
        .Build());
    services.AddSingleton<IAuthorizationHandler, KrakenAdminAuthorizationHandler>();

    IEnumerable<ServiceDescriptor> cookiesSettingsDescriptors = services.Where(service => service.ServiceType.Equals(typeof(CookiesSettings)));
    CookiesSettings cookiesSettings;
    if (cookiesSettingsDescriptors.Any())
    {
      CookiesSettings[] cookiesSettingsValues = cookiesSettingsDescriptors
        .Where(descriptor => descriptor.ImplementationInstance is CookiesSettings)
        .Select(descriptor => (CookiesSettings)descriptor.ImplementationInstance!)
        .ToArray();
      if (cookiesSettingsValues.Length < 1)
      {
        throw new InvalidOperationException($"No {nameof(CookiesSettings)} implementation instance was registered.");
      }
      else if (cookiesSettingsValues.Length > 1)
      {
        throw new InvalidOperationException($"More than one {nameof(CookiesSettings)} implementation instances were registered.");
      }
      cookiesSettings = cookiesSettingsValues.Single();
    }
    else
    {
      cookiesSettings = CookiesSettings.Initialize(_configuration);
      services.AddSingleton(cookiesSettings);
    }
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();
    services.AddOpenApi();

    services.AddDistributedMemoryCache();
    //services.AddExceptionHandler<ExceptionHandler>(); // TODO(fpion): ExceptionHandler
    services.AddFeatureManagement();
    //services.AddProblemDetails(); // TODO(fpion): ExceptionHandler

    DatabaseProvider databaseProvider = EnvironmentHelper.GetEnum(
      "DATABASE_PROVIDER",
      _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer);
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
    application.UseSession();
    application.UseMiddleware<RenewSession>();
    //application.UseMiddleware<RedirectNotFound>(); // TODO(fpion): Frontend Integration
    application.UseAuthentication();
    application.UseAuthorization();

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
