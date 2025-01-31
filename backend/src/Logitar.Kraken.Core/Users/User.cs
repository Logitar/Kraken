using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Users.Events;

namespace Logitar.Kraken.Core.Users;

public class User : AggregateRoot
{
  private Password? _password = null;

  public new UserId Id => new(base.Id);

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The user has not been initialized.");

  public bool HasPassword => _password != null;

  public string? FullName { get; }

  public User(UniqueName uniqueName, ActorId? actorId = null, UserId? userId = null) : base(userId?.StreamId)
  {
    Raise(new UserCreated(uniqueName), actorId);
  }
  protected virtual void Handle(UserCreated @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void SetPassword(Password password, ActorId? actorId = null)
  {
    Raise(new UserPasswordChanged(password), actorId);
  }
  protected virtual void Handle(UserPasswordChanged @event)
  {
    _password = @event.Password;
  }

  public override string ToString() => $"{FullName ?? UniqueName.Value} | {base.ToString()}";
}
