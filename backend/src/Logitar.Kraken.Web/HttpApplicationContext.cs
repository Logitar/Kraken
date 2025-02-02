using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Web;

internal class HttpApplicationContext : IApplicationContext // TODO(fpion): Authentication
{
  public ActorId? ActorId => throw new NotImplementedException();
  public RealmId? RealmId => throw new NotImplementedException();
  public IRoleSettings RoleSettings => throw new NotImplementedException();
}
