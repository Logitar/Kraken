using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class OneTimePasswords
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.OneTimePasswords), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(OneTimePasswordEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(OneTimePasswordEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(OneTimePasswordEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(OneTimePasswordEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(OneTimePasswordEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(OneTimePasswordEntity.Version), Table);

  public static readonly ColumnId AttemptCount = new(nameof(OneTimePasswordEntity.AttemptCount), Table);
  public static readonly ColumnId CustomAttributes = new(nameof(OneTimePasswordEntity.CustomAttributes), Table);
  public static readonly ColumnId ExpiresOn = new(nameof(OneTimePasswordEntity.ExpiresOn), Table);
  public static readonly ColumnId HasValidationSucceeded = new(nameof(OneTimePasswordEntity.HasValidationSucceeded), Table);
  public static readonly ColumnId Id = new(nameof(OneTimePasswordEntity.Id), Table);
  public static readonly ColumnId MaximumAttempts = new(nameof(OneTimePasswordEntity.MaximumAttempts), Table);
  public static readonly ColumnId OneTimePasswordId = new(nameof(OneTimePasswordEntity.OneTimePasswordId), Table);
  public static readonly ColumnId PasswordHash = new(nameof(OneTimePasswordEntity.PasswordHash), Table);
  public static readonly ColumnId RealmId = new(nameof(OneTimePasswordEntity.RealmId), Table);
  public static readonly ColumnId UserId = new(nameof(OneTimePasswordEntity.UserId), Table);
}
