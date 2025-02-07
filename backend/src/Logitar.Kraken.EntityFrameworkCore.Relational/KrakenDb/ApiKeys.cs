using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class ApiKeys
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.ApiKeys), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(ApiKeyEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ApiKeyEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(ApiKeyEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ApiKeyEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ApiKeyEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ApiKeyEntity.Version), Table);

  public static readonly ColumnId ApiKeyId = new(nameof(ApiKeyEntity.ApiKeyId), Table);
  public static readonly ColumnId AuthenticatedOn = new(nameof(ApiKeyEntity.AuthenticatedOn), Table);
  public static readonly ColumnId CustomAttributes = new(nameof(ApiKeyEntity.CustomAttributes), Table);
  public static readonly ColumnId Description = new(nameof(ApiKeyEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(ApiKeyEntity.Name), Table);
  public static readonly ColumnId ExpiresOn = new(nameof(ApiKeyEntity.ExpiresOn), Table);
  public static readonly ColumnId Id = new(nameof(ApiKeyEntity.Id), Table);
  public static readonly ColumnId RealmId = new(nameof(ApiKeyEntity.RealmId), Table);
  public static readonly ColumnId SecretHash = new(nameof(ApiKeyEntity.SecretHash), Table);
}
