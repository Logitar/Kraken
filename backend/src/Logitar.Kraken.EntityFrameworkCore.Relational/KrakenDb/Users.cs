using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Users
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.Users), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(UserEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(UserEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(UserEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(UserEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(UserEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(UserEntity.Version), Table);

  public static readonly ColumnId AddressCountry = new(nameof(UserEntity.AddressCountry), Table);
  public static readonly ColumnId AddressFormatted = new(nameof(UserEntity.AddressFormatted), Table);
  public static readonly ColumnId AddressLocality = new(nameof(UserEntity.AddressLocality), Table);
  public static readonly ColumnId AddressPostalCode = new(nameof(UserEntity.AddressPostalCode), Table);
  public static readonly ColumnId AddressRegion = new(nameof(UserEntity.AddressRegion), Table);
  public static readonly ColumnId AddressStreet = new(nameof(UserEntity.AddressStreet), Table);
  public static readonly ColumnId AddressVerifiedBy = new(nameof(UserEntity.AddressVerifiedBy), Table);
  public static readonly ColumnId AddressVerifiedOn = new(nameof(UserEntity.AddressVerifiedOn), Table);
  public static readonly ColumnId AuthenticatedOn = new(nameof(UserEntity.AuthenticatedOn), Table);
  public static readonly ColumnId Birthdate = new(nameof(UserEntity.Birthdate), Table);
  public static readonly ColumnId CustomAttributes = new(nameof(UserEntity.CustomAttributes), Table);
  public static readonly ColumnId DisabledBy = new(nameof(UserEntity.DisabledBy), Table);
  public static readonly ColumnId DisabledOn = new(nameof(UserEntity.DisabledOn), Table);
  public static readonly ColumnId EmailAddress = new(nameof(UserEntity.EmailAddress), Table);
  public static readonly ColumnId EmailAddressNormalized = new(nameof(UserEntity.EmailAddressNormalized), Table);
  public static readonly ColumnId EmailVerifiedBy = new(nameof(UserEntity.EmailVerifiedBy), Table);
  public static readonly ColumnId EmailVerifiedOn = new(nameof(UserEntity.EmailVerifiedOn), Table);
  public static readonly ColumnId FirstName = new(nameof(UserEntity.FirstName), Table);
  public static readonly ColumnId FullName = new(nameof(UserEntity.FullName), Table);
  public static readonly ColumnId Gender = new(nameof(UserEntity.Gender), Table);
  public static readonly ColumnId HasPassword = new(nameof(UserEntity.HasPassword), Table);
  public static readonly ColumnId Id = new(nameof(UserEntity.Id), Table);
  public static readonly ColumnId IsAddressVerified = new(nameof(UserEntity.IsAddressVerified), Table);
  public static readonly ColumnId IsConfirmed = new(nameof(UserEntity.IsConfirmed), Table);
  public static readonly ColumnId IsDisabled = new(nameof(UserEntity.IsDisabled), Table);
  public static readonly ColumnId IsEmailVerified = new(nameof(UserEntity.IsEmailVerified), Table);
  public static readonly ColumnId IsPhoneVerified = new(nameof(UserEntity.IsPhoneVerified), Table);
  public static readonly ColumnId LastName = new(nameof(UserEntity.LastName), Table);
  public static readonly ColumnId Locale = new(nameof(UserEntity.Locale), Table);
  public static readonly ColumnId MiddleName = new(nameof(UserEntity.MiddleName), Table);
  public static readonly ColumnId Nickname = new(nameof(UserEntity.Nickname), Table);
  public static readonly ColumnId PasswordChangedBy = new(nameof(UserEntity.PasswordChangedBy), Table);
  public static readonly ColumnId PasswordChangedOn = new(nameof(UserEntity.PasswordChangedOn), Table);
  public static readonly ColumnId PasswordHash = new(nameof(UserEntity.PasswordHash), Table);
  public static readonly ColumnId PhoneCountryCode = new(nameof(UserEntity.PhoneCountryCode), Table);
  public static readonly ColumnId PhoneE164Formatted = new(nameof(UserEntity.PhoneE164Formatted), Table);
  public static readonly ColumnId PhoneExtension = new(nameof(UserEntity.PhoneExtension), Table);
  public static readonly ColumnId PhoneNumber = new(nameof(UserEntity.PhoneNumber), Table);
  public static readonly ColumnId PhoneVerifiedBy = new(nameof(UserEntity.PhoneVerifiedBy), Table);
  public static readonly ColumnId PhoneVerifiedOn = new(nameof(UserEntity.PhoneVerifiedOn), Table);
  public static readonly ColumnId Picture = new(nameof(UserEntity.Picture), Table);
  public static readonly ColumnId Profile = new(nameof(UserEntity.Profile), Table);
  public static readonly ColumnId RealmId = new(nameof(UserEntity.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(UserEntity.RealmUid), Table);
  public static readonly ColumnId TimeZone = new(nameof(UserEntity.TimeZone), Table);
  public static readonly ColumnId UniqueName = new(nameof(UserEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(UserEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId UserId = new(nameof(UserEntity.UserId), Table);
  public static readonly ColumnId Website = new(nameof(UserEntity.Website), Table);
}
