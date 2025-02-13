using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using MediatR;

namespace Logitar.Kraken.Core.Sessions.Events;

public record SessionRenewed(Password Secret) : DomainEvent, INotification;
