using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Sessions.Events;

public record SessionCreated(UserId UserId, Password? Secret) : DomainEvent, INotification;
