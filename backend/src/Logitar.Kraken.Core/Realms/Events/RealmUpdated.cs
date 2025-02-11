using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Events;

public record RealmUpdated : DomainEvent, INotification
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  // TODO(fpion): Secret
  public Change<Url>? Url { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }
  public bool? RequireConfirmedAccount { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null
    || Url != null
    || UniqueNameSettings != null || PasswordSettings != null || RequireUniqueEmail != null || RequireConfirmedAccount != null
    || CustomAttributes.Count > 0;
}
