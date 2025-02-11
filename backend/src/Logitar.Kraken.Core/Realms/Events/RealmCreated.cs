using Logitar.EventSourcing;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Events;

public record RealmCreated(Slug UniqueSlug, Secret Secret) : DomainEvent, INotification;
