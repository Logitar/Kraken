using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserUpdated : DomainEvent, INotification
{
  public Change<PersonName>? FirstName { get; set; }
  public Change<PersonName>? MiddleName { get; set; }
  public Change<PersonName>? LastName { get; set; }
  public Change<string>? FullName { get; set; }
  public Change<PersonName>? Nickname { get; set; }

  public Change<Url>? Picture { get; set; }
  public Change<Url>? Profile { get; set; }
  public Change<Url>? Website { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => FirstName != null || MiddleName != null || LastName != null || FullName != null || Nickname != null
    || Picture != null || Profile != null || Website != null
    || CustomAttributes.Count > 0;
}
