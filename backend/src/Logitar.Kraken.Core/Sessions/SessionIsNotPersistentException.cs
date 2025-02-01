namespace Logitar.Kraken.Core.Sessions;

public class SessionIsNotPersistentException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified session is not persistent.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid SessionId
  {
    get => (Guid)Data[nameof(SessionId)]!;
    private set => Data[nameof(SessionId)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(SessionId)] = SessionId;
      return error;
    }
  }

  public SessionIsNotPersistentException(Session session) : base(BuildMessage(session))
  {
    RealmId = session.Id.RealmId?.ToGuid();
    SessionId = session.Id.EntityId;
  }

  private static string BuildMessage(Session session) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), session.Id.RealmId?.ToGuid())
    .AddData(nameof(SessionId), session.Id.EntityId)
    .Build();
}
