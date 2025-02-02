﻿using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Events;

public record ConfigurationUpdated : DomainEvent, INotification
{
  public JwtSecret? Secret { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }

  public LoggingSettings? LoggingSettings { get; set; }

  [JsonIgnore]
  public bool HasChanges => Secret != null || UniqueNameSettings != null || PasswordSettings != null || LoggingSettings != null;
}
