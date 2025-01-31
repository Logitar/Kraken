using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Events;

public record RealmCreated(
  Slug UniqueSlug,
  JwtSecret Secret,
  UniqueNameSettings UniqueNameSettings,
  PasswordSettings PasswordSettings,
  bool RequireUniqueEmail,
  bool RequireConfirmedAccount) : DomainEvent, INotification;
