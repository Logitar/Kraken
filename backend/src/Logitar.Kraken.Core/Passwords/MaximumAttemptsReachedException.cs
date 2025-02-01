namespace Logitar.Kraken.Core.Passwords;

public class MaximumAttemptsReachedException : InvalidCredentialsException
{
  private const string ErrorMessage = "The maximum number of attempts has been reached for this One-Time Password (OTP).";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid OneTimePasswordId
  {
    get => (Guid)Data[nameof(OneTimePasswordId)]!;
    private set => Data[nameof(OneTimePasswordId)] = value;
  }
  public int AttemptCount
  {
    get => (int)Data[nameof(AttemptCount)]!;
    private set => Data[nameof(AttemptCount)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(OneTimePasswordId)] = OneTimePasswordId;
      error.Data[nameof(AttemptCount)] = AttemptCount;
      return error;
    }
  }

  public MaximumAttemptsReachedException(OneTimePassword oneTimePassword, int attemptCount)
    : base(BuildMessage(oneTimePassword, attemptCount))
  {
    RealmId = oneTimePassword.Id.RealmId?.ToGuid();
    OneTimePasswordId = oneTimePassword.Id.EntityId;
    AttemptCount = attemptCount;
  }

  private static string BuildMessage(OneTimePassword oneTimePassword, int attemptCount) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), oneTimePassword.Id.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(OneTimePasswordId), oneTimePassword.Id.EntityId)
    .AddData(nameof(AttemptCount), attemptCount)
    .Build();
}
