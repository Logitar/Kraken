using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Web;

internal class HttpApplicationContext : IApplicationContext // TODO(fpion): authentication
{
  public ActorId? ActorId => throw new NotImplementedException();

  public RealmModel? Realm => throw new NotImplementedException();
  public RealmId? RealmId => throw new NotImplementedException();
}
