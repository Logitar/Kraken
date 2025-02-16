using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Web.Models.Account;

public record UpdateProfilePayload
{
  public ChangeAccountPassword? Password { get; set; }

  public ChangeModel<AddressPayload>? Address { get; set; }
  public ChangeModel<EmailPayload>? Email { get; set; }
  public ChangeModel<PhonePayload>? Phone { get; set; }

  public ChangeModel<string>? FirstName { get; set; }
  public ChangeModel<string>? MiddleName { get; set; }
  public ChangeModel<string>? LastName { get; set; }
  public ChangeModel<string>? Nickname { get; set; }

  public ChangeModel<DateTime?>? Birthdate { get; set; }
  public ChangeModel<string>? Gender { get; set; }
  public ChangeModel<string>? Locale { get; set; }
  public ChangeModel<string>? TimeZone { get; set; }

  public ChangeModel<string>? Picture { get; set; }
  public ChangeModel<string>? Profile { get; set; }
  public ChangeModel<string>? Website { get; set; }

  public UpdateUserPayload ToUpdateUserPayload() => new()
  {
    Password = Password?.ToChangePasswordPayload(),
    Address = Address,
    Email = Email,
    Phone = Phone,
    FirstName = FirstName,
    MiddleName = MiddleName,
    LastName = LastName,
    Nickname = Nickname,
    Birthdate = Birthdate,
    Gender = Gender,
    Locale = Locale,
    TimeZone = TimeZone,
    Picture = Picture,
    Profile = Profile,
    Website = Website
  };
}
