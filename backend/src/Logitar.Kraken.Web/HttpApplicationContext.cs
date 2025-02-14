using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Web;

internal class HttpApplicationContext : IApplicationContext // TODO(fpion): authentication
{
  public ActorId? ActorId => throw new NotImplementedException();

  public string BaseUrl => throw new NotImplementedException();

  public RealmModel? Realm => throw new NotImplementedException();
  public RealmId? RealmId => throw new NotImplementedException();
  public string Secret => throw new NotImplementedException();
  public IUserSettings UserSettings => throw new NotImplementedException();
  public IUniqueNameSettings UniqueNameSettings => throw new NotImplementedException();
}
