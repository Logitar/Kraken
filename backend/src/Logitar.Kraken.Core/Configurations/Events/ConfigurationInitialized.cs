using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Events;

public record ConfigurationInitialized(Secret Secret, UniqueNameSettings UniqueNameSettings, PasswordSettings PasswordSettings) : DomainEvent, INotification;
