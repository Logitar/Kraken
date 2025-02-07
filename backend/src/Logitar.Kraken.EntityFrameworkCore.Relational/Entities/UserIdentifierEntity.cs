using Logitar.Kraken.Core.Users.Events;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class UserIdentifierEntity : IdentifierEntity
{
  public int UserIdentifierId { get; private set; }

  public UserEntity? User { get; private set; }
  public int UserId { get; private set; }

  public UserIdentifierEntity(UserEntity user, UserIdentifierChanged @event) : base(user.Realm, @event.Key.Value)
  {
    User = user;
    UserId = user.UserId;

    Update(@event);
  }

  private UserIdentifierEntity() : base()
  {
  }

  public void Update(UserIdentifierChanged @event)
  {
    Value = @event.Value.Value;
  }

  public override bool Equals(object? obj) => obj is UserIdentifierEntity identifier && identifier.UserIdentifierId == UserIdentifierId;
  public override int GetHashCode() => UserIdentifierId.GetHashCode();
  public override string ToString() => $"{GetType()} (UserIdentifierId={UserIdentifierId})";
}
