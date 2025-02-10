﻿using Logitar.EventSourcing.Infrastructure;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Messages;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Tokens;
using Logitar.Kraken.Infrastructure.Caching;
using Logitar.Kraken.Infrastructure.Converters;
using Logitar.Kraken.Infrastructure.Messages;
using Logitar.Kraken.Infrastructure.Passwords;
using Logitar.Kraken.Infrastructure.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenInfrastructure(this IServiceCollection services)
  {
    return services
      .AddMemoryCache()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddSingleton<IMessageManager, MessageManager>()
      .AddSingleton<IPasswordManager, PasswordManager>()
      .AddSingleton<PasswordConverter>()
      .AddScoped<IEventBus, EventBus>()
      .AddScoped<ITokenManager, JsonWebTokenManager>();
  }
}
