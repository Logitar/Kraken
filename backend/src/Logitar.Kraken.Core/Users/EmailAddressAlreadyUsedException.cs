namespace Logitar.Kraken.Core.Users;

public class EmailAddressAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified email address is already used.";

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
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string EmailAddress
  {
    get => (string)Data[nameof(EmailAddress)]!;
    private set => Data[nameof(EmailAddress)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(UserId)] = UserId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(EmailAddress)] = EmailAddress;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public EmailAddressAlreadyUsedException(User user, UserId conflictId) : base(BuildMessage(user, conflictId))
  {
    RealmId = user.RealmId?.ToGuid();
    UserId = user.EntityId;
    ConflictId = conflictId.EntityId;
    EmailAddress = user.Email?.Address ?? throw new ArgumentException($"The {nameof(user.Email)} is required.", nameof(user));
    PropertyName = nameof(user.Email);
  }

  private static string BuildMessage(User user, UserId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), user.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(ConflictId), conflictId.EntityId)
    .AddData(nameof(EmailAddress), user.Email)
    .AddData(nameof(PropertyName), nameof(user.Email))
    .Build();
}
