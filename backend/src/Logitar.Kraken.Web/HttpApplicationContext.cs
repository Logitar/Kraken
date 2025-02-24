using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using Logitar.Kraken.Web.Extensions;

namespace Logitar.Kraken.Web;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  private ConfigurationModel Configuration => _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");

  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }

  public ActorId? ActorId
  {
    get
    {
      if (_httpContextAccessor.HttpContext != null)
      {
        UserModel? user = _httpContextAccessor.HttpContext.GetUser();
        if (user != null)
        {
          return new ActorId(user.Id);
        }

        ApiKeyModel? apiKey = _httpContextAccessor.HttpContext.GetApiKey();
        if (apiKey != null)
        {
          return new ActorId(apiKey.Id);
        }
      }

      return null;
    }
  }

  public string BaseUrl
  {
    get
    {
      HttpContext context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");
      return new Uri($"{context.Request.Scheme}://{context.Request.Host}", UriKind.Absolute).ToString().Trim('/');
    }
  }

  public RealmModel? Realm => _httpContextAccessor.HttpContext?.GetRealm();
  public RealmId? RealmId => Realm == null ? null : new RealmId(Realm.Id);
  public Secret Secret => new(Realm?.Secret ?? Configuration.Secret);
  public IUserSettings UserSettings => new UserSettings(UniqueNameSettings, Realm?.PasswordSettings ?? Configuration.PasswordSettings, Realm?.RequireUniqueEmail ?? false, Realm?.RequireConfirmedAccount ?? false);
  public IUniqueNameSettings UniqueNameSettings => Realm?.UniqueNameSettings ?? Configuration.UniqueNameSettings;
}
