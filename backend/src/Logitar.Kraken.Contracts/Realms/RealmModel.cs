using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Contracts.Realms;

public class RealmModel : AggregateModel
{
  public string UniqueSlug { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Secret { get; set; } = string.Empty;
  public string? Url { get; set; }

  public UniqueNameSettingsModel UniqueNameSettings { get; set; } = new();
  public PasswordSettingsModel PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }
  public bool RequireConfirmedAccount { get; set; }
}
