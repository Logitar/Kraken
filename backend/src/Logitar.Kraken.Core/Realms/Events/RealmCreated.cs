using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Events;

public record RealmCreated(Slug UniqueSlug) : DomainEvent, INotification; // TODO(fpion): Secret
