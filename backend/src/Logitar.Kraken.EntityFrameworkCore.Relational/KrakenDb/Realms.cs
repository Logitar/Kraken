using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Realms
{
  public static readonly TableId Table = new(Schemas.Kraken, nameof(KrakenContext.Realms), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(RealmEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(RealmEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(RealmEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(RealmEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(RealmEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(RealmEntity.Version), Table);

  public static readonly ColumnId AllowedUniqueNameCharacters = new(nameof(RealmEntity.AllowedUniqueNameCharacters), Table);
  public static readonly ColumnId CustomAttributes = new(nameof(RealmEntity.CustomAttributes), Table);
  public static readonly ColumnId Description = new(nameof(RealmEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(RealmEntity.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(RealmEntity.Id), Table);
  public static readonly ColumnId PasswordHashingStrategy = new(nameof(RealmEntity.PasswordHashingStrategy), Table);
  public static readonly ColumnId PasswordsRequireDigit = new(nameof(RealmEntity.PasswordsRequireDigit), Table);
  public static readonly ColumnId PasswordsRequireLowercase = new(nameof(RealmEntity.PasswordsRequireLowercase), Table);
  public static readonly ColumnId PasswordsRequireNonAlphanumeric = new(nameof(RealmEntity.PasswordsRequireNonAlphanumeric), Table);
  public static readonly ColumnId PasswordsRequireUppercase = new(nameof(RealmEntity.PasswordsRequireUppercase), Table);
  public static readonly ColumnId RealmId = new(nameof(RealmEntity.RealmId), Table);
  public static readonly ColumnId RequireConfirmedAccount = new(nameof(RealmEntity.RequireConfirmedAccount), Table);
  public static readonly ColumnId RequiredPasswordLength = new(nameof(RealmEntity.RequiredPasswordLength), Table);
  public static readonly ColumnId RequiredPasswordUniqueChars = new(nameof(RealmEntity.RequiredPasswordUniqueChars), Table);
  public static readonly ColumnId RequireUniqueEmail = new(nameof(RealmEntity.RequireUniqueEmail), Table);
  public static readonly ColumnId Secret = new(nameof(RealmEntity.Secret), Table);
  public static readonly ColumnId UniqueSlug = new(nameof(RealmEntity.UniqueSlug), Table);
  public static readonly ColumnId UniqueSlugNormalized = new(nameof(RealmEntity.UniqueSlugNormalized), Table);
  public static readonly ColumnId Url = new(nameof(RealmEntity.Url), Table);
}
