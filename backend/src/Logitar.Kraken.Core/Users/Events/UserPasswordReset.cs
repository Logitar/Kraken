using Logitar.Kraken.Core.Passwords;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserPasswordReset : UserPasswordEvent, INotification
{
  public UserPasswordReset(Password password) : base(password)
  {
  }
}
