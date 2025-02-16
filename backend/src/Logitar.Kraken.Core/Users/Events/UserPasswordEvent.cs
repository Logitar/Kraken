using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Core.Users.Events;

public abstract record UserPasswordEvent(Password Password) : DomainEvent;
