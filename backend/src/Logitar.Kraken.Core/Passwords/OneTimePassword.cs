using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords.Events;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Passwords;

public class OneTimePassword : AggregateRoot
{
  public new OneTimePasswordId Id => new(base.Id);

  public OneTimePassword() : base()
  {
  }

  public OneTimePassword(Password password, User? user = null, ActorId? actorId = null, OneTimePasswordId? oneTimePasswordId = null)
    : base(oneTimePasswordId?.StreamId)
  {
    // TODO(fpion): implement
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new OneTimePasswordDeleted(), actorId);
    }
  }
}
