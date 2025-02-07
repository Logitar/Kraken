using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Kraken.Infrastructure.Caching;

public class CacheService : ICacheService
{
  private const string ConfigurationKey = "Configuration";

  protected IMemoryCache Cache { get; }

  public CacheService(IMemoryCache cache)
  {
    Cache = cache;
  }

  public virtual ConfigurationModel? Configuration
  {
    get => TryGetValue<ConfigurationModel>(ConfigurationKey);
    set
    {
      if (value == null)
      {
        RemoveValue(ConfigurationKey);
      }
      else
      {
        SetValue(ConfigurationKey, value);
      }
    }
  }

  protected T? TryGetValue<T>(object key) => Cache.TryGetValue(key, out object? value) ? (T?)value : default;
  protected void RemoveValue(object key) => Cache.Remove(key);
  protected void SetValue<T>(object key, T value, TimeSpan? duration = null)
  {
    if (duration.HasValue)
    {
      Cache.Set(key, value, duration.Value);
    }
    else
    {
      Cache.Set(key, value);
    }
  }
}
