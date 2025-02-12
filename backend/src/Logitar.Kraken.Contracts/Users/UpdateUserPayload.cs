using Logitar.Kraken.Contracts.Roles;

namespace Logitar.Kraken.Contracts.Users;

public record UpdateUserPayload
{
  public string? UniqueName { get; set; }
  public ChangePasswordPayload? Password { get; set; }
  public bool? IsDisabled { get; set; }

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

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
  public List<RoleAction> Roles { get; set; } = [];
}
