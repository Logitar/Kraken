using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Sessions;
using Logitar.Kraken.Core.Sessions.Events;
using System.Text.Json;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class SessionEntity : AggregateEntity
{
  public int SessionId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public Guid Id { get; private set; }

  public UserEntity? User { get; private set; }
  public int UserId { get; private set; }

  public string? SecretHash { get; private set; }
  public bool IsPersistent
  {
    get => SecretHash != null;
    private set { }
  }

  public string? SignedOutBy { get; private set; }
  public DateTime? SignedOutOn { get; private set; }
  public bool IsActive
  {
    get => !SignedOutOn.HasValue;
    private set { }
  }

  public string? CustomAttributes { get; private set; }

  public SessionEntity(UserEntity user, SessionCreated @event) : base(@event)
  {
    Realm = user.Realm;
    RealmId = user.RealmId;

    SessionId sessionId = new(@event.StreamId);
    Id = sessionId.EntityId;

    User = user;
    UserId = user.UserId;

    SecretHash = @event.Secret?.Encode();
  }

  private SessionEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipUser: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipUser)
  {
    List<ActorId> actorIds = [];
    actorIds.AddRange(base.GetActorIds());

    if (SignedOutBy != null)
    {
      actorIds.Add(new ActorId(SignedOutBy));
    }

    if (!skipUser && User != null)
    {
      actorIds.AddRange(User.GetActorIds(skipRoles: false, skipSessions: true));
    }

    return actorIds.AsReadOnly();
  }

  public void Renew(SessionRenewed @event)
  {
    Update(@event);

    SecretHash = @event.Secret.Encode();
  }

  public void SignOut(SessionSignedOut @event)
  {
    Update(@event);

    SignedOutBy = @event.ActorId?.Value;
    SignedOutOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Update(SessionUpdated @event)
  {
    base.Update(@event);

    Dictionary<string, string> customAttributes = GetCustomAttributes();
    foreach (KeyValuePair<Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        customAttributes.Remove(customAttribute.Key.Value);
      }
      else
      {
        customAttributes[customAttribute.Key.Value] = customAttribute.Value;
      }
    }
    SetCustomAttributes(customAttributes);
  }

  public Dictionary<string, string> GetCustomAttributes()
  {
    return (CustomAttributes == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(CustomAttributes)) ?? [];
  }
  private void SetCustomAttributes(Dictionary<string, string> customAttributes)
  {
    CustomAttributes = customAttributes.Count < 1 ? null : JsonSerializer.Serialize(customAttributes);
  }
}
