using Logitar.EventSourcing.Infrastructure;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Tokens;
using Logitar.Kraken.Infrastructure.Caching;
using Logitar.Kraken.Infrastructure.Converters;
using Logitar.Kraken.Infrastructure.Passwords;
using Logitar.Kraken.Infrastructure.Passwords.Pbkdf2;
using Logitar.Kraken.Infrastructure.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenInfrastructure(this IServiceCollection services)
  {
    return services
      .AddMemoryCache()
      .AddPasswordStrategies()
      .AddSingleton(serviceProvider => Pbkdf2Settings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddSingleton<IPasswordManager, PasswordManager>()
      .AddSingleton<ISecretHelper, SecretHelper>()
      .AddSingleton<PasswordConverter>()
      .AddScoped<IEventBus, EventBus>()
      .AddScoped<ITokenManager, JsonWebTokenManager>();
  }

  private static IServiceCollection AddPasswordStrategies(this IServiceCollection services)
  {
    return services.AddSingleton<IPasswordStrategy, Pbkdf2Strategy>();
  }
}
