using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Events;

public record RealmUpdated : DomainEvent, INotification
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Secret? Secret { get; set; }
  public Change<Url>? Url { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }
  public bool? RequireConfirmedAccount { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null
    || Secret != null || Url != null
    || UniqueNameSettings != null || PasswordSettings != null || RequireUniqueEmail != null || RequireConfirmedAccount != null
    || CustomAttributes.Count > 0;
}
