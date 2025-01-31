﻿using Logitar.Kraken.Core.Roles;

namespace Logitar.Kraken.Core;

public class UniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique name is already used.";

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
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
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
      return error;
    }
  }

  public UniqueNameAlreadyUsedException(Role role, RoleId conflictId)
    : this(role.Id.ToGuid(), conflictId.ToGuid(), role.UniqueName.Value, nameof(role.UniqueName))
  {
  }
  private UniqueNameAlreadyUsedException(Guid entityId, Guid conflictId, string uniqueName, string propertyName)
    : base(BuildMessage(entityId, conflictId, uniqueName, propertyName))
  {
  }

  private static string BuildMessage(Guid entityId, Guid conflictId, string uniqueName, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(EntityId), entityId)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(UniqueName), uniqueName)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
