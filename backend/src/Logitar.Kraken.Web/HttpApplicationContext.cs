using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Web;

internal class HttpApplicationContext : IApplicationContext // TODO(fpion): authentication
{
  public ActorId? ActorId => null;

  public string BaseUrl => throw new NotImplementedException();

  public RealmModel? Realm => null;
  public RealmId? RealmId => null;
  public Secret Secret => throw new NotImplementedException();
  public IUserSettings UserSettings => throw new NotImplementedException();
  public IUniqueNameSettings UniqueNameSettings => throw new NotImplementedException();
}
