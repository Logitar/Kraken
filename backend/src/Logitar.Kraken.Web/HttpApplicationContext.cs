using Logitar.EventSourcing;
using Logitar.Kraken.Core;

namespace Logitar.Kraken.Web;

internal class HttpApplicationContext : IApplicationContext
{
  public ActorId? ActorId => throw new NotImplementedException(); // TODO(fpion): authentication
}
