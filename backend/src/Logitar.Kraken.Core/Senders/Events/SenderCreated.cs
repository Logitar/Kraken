using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Events;

public record SenderCreated(Email? Email, Phone? Phone, SenderProvider Provider) : DomainEvent, INotification;
