using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Sessions;

namespace Logitar.Kraken.Contracts.Users;

public class UserModel : AggregateModel
{
  public string UniqueName { get; set; } = string.Empty;

  public bool HasPassword { get; set; }
  public ActorModel? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public ActorModel? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

  public AddressModel? Address { get; set; }
  public EmailModel? Email { get; set; }
  public PhoneModel? Phone { get; set; }
  public bool IsConfirmed { get; set; }

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }
  public string? Nickname { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public LocaleModel? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }
  public string? Profile { get; set; }
  public string? Website { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
  public List<CustomIdentifierModel> CustomIdentifiers { get; set; } = [];
  public List<RoleModel> Roles { get; set; } = [];
  public List<SessionModel> Sessions { get; set; } = [];

  public RealmModel? Realm { get; set; }

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
