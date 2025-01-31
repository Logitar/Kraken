using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Events;

public record RealmUniqueSlugChanged(Slug UniqueSlug) : DomainEvent, INotification;
