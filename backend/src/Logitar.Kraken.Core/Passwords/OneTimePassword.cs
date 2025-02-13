using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords.Events;

namespace Logitar.Kraken.Core.Passwords;

public class OneTimePassword : AggregateRoot
{
  public new OneTimePasswordId Id => new(base.Id);

  public OneTimePassword() : base()
  {
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new OneTimePasswordDeleted(), actorId);
    }
  }
}
