using Logitar.Kraken.Core.Passwords;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserPasswordUpdated : UserPasswordEvent, INotification
{
  public UserPasswordUpdated(Password password) : base(password)
  {
  }
}
