﻿using Logitar.EventSourcing;
using Logitar.Kraken.Core.Roles;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserRoleRemoved(RoleId RoleId) : DomainEvent, INotification;
