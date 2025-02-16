using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Contracts.Realms;

public class RealmModel : AggregateModel
{
  public string UniqueSlug { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  [JsonIgnore]
  public string Secret { get; set; } = string.Empty;

  public string? Url { get; set; }

  public UniqueNameSettingsModel UniqueNameSettings { get; set; } = new();
  public PasswordSettingsModel PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }
  public bool RequireConfirmedAccount { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
