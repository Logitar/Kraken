using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Sessions.Events;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Sessions;

public class Session : AggregateRoot // TODO(fpion): tests
{
  public new SessionId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public UserId UserId { get; private set; }

  private Password? _secret = null;
  public bool IsPersistent => _secret != null;

  public bool IsActive { get; private set; }

  public Session() : base()
  {
  }

  public Session(User user, Password? secret = null, ActorId? actorId = null, SessionId? sessionId = null)
    : base(sessionId?.StreamId)
  {
    // TODO(fpion): ensure user is in same realm

    Raise(new SessionCreated(user.Id, secret), actorId);
  }
  protected virtual void Handle(SessionCreated @event)
  {
    UserId = @event.UserId;

    _secret = @event.Secret;

    IsActive = true;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SessionDeleted(), actorId);
    }
  }

  public void SignOut(ActorId? actorId = null)
  {
    if (IsActive)
    {
      Raise(new SessionSignedOut(), actorId);
    }
  }
  protected virtual void Handle(SessionSignedOut _)
  {
    IsActive = false;
  }
}
