using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core;

public class CustomIdentifierAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified custom identifier is already used.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string Key
  {
    get => (string)Data[nameof(Key)]!;
    private set => Data[nameof(Key)] = value;
  }
  public string Value
  {
    get => (string)Data[nameof(Value)]!;
    private set => Data[nameof(Value)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(EntityId)] = EntityId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(Key)] = Key;
      error.Data[nameof(Value)] = Value;
      return error;
    }
  }

  public CustomIdentifierAlreadyUsedException(User user, UserId conflict, Identifier key, CustomIdentifier value)
    : this(typeof(User), user.RealmId, conflict.EntityId, user.EntityId, key, value)
  {
  }

  public CustomIdentifierAlreadyUsedException(Type type, RealmId? realmId, Guid conflictId, Guid entityId, Identifier key, CustomIdentifier value)
    : base(BuildMessage(type, realmId, conflictId, entityId, key, value))
  {
    TypeName = type.GetNamespaceQualifiedName();
    RealmId = realmId?.ToGuid();
    EntityId = entityId;
    ConflictId = conflictId;
    Key = key.Value;
    Value = value.Value;
  }

  private static string BuildMessage(Type type, RealmId? realmId, Guid conflictId, Guid entityId, Identifier key, CustomIdentifier value) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
    .AddData(nameof(RealmId), realmId?.Value, "<null>")
    .AddData(nameof(EntityId), entityId)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(Key), key)
    .AddData(nameof(Value), value)
    .Build();
}
