using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Web;

internal class HttpApplicationContext : IApplicationContext // ISSUE #35: https://github.com/Logitar/Kraken/issues/35
{
  public ActorId? ActorId => throw new NotImplementedException();

  public ConfigurationModel Configuration => throw new NotImplementedException();
  public RealmModel? Realm => throw new NotImplementedException();
  public RealmId? RealmId => throw new NotImplementedException();
  public string BaseUrl => throw new NotImplementedException();
  public string Secret => throw new NotImplementedException();
  public IRoleSettings RoleSettings => throw new NotImplementedException();
  public IUserSettings UserSettings => throw new NotImplementedException();
}
