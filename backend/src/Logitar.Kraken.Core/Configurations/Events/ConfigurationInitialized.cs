using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Events;

public record ConfigurationInitialized(
  UniqueNameSettings UniqueNameSettings,
  PasswordSettings PasswordSettings) : DomainEvent, INotification;
