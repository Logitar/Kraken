using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserSignedIn : DomainEvent, INotification
{
  public UserSignedIn(DateTime signedInOn)
  {
    OccurredOn = signedInOn;
  }
}
