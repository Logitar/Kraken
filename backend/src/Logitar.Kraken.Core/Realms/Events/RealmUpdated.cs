using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Events;

public record RealmUpdated : DomainEvent, INotification
{
  public Slug? UniqueName { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public JwtSecret? Secret { get; set; }
  public Change<Url>? Url { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }
  public bool? RequireConfirmedAccount { get; set; }

  [JsonIgnore]
  public bool HasChanges => UniqueName != null || DisplayName != null || Description != null
    || Secret != null || Url != null
    || UniqueNameSettings != null || PasswordSettings != null || RequireUniqueEmail != null || RequireConfirmedAccount != null;
}
