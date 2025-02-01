﻿namespace Logitar.Kraken.Core.Users;

public class UserHasNoPasswordException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified user has no password.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid UserId
  {
    get => (Guid)Data[nameof(UserId)]!;
    private set => Data[nameof(UserId)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(UserId)] = UserId;
      return error;
    }
  }

  public UserHasNoPasswordException(User user) : base(BuildMessage(user))
  {
    RealmId = user.Id.RealmId?.ToGuid();
    UserId = user.Id.EntityId;
  }

  private static string BuildMessage(User user) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), user.Id.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(UserId), user.Id.EntityId)
    .Build();
}
