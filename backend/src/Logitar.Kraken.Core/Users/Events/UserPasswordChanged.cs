using Logitar.Kraken.Core.Passwords;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserPasswordChanged : UserPasswordEvent, INotification
{
  public UserPasswordChanged(Password password) : base(password)
  {
  }
}
