using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Contracts.Realms;

public record UpdateRealmPayload
{
  public string? UniqueSlug { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public string? Secret { get; set; }
  public ChangeModel<string>? Url { get; set; }

  public UniqueNameSettingsModel? UniqueNameSettings { get; set; }
  public PasswordSettingsModel? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }
  public bool? RequireConfirmedAccount { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
}
