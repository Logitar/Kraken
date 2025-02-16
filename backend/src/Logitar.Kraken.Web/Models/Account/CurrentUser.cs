using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Web.Models.Account;

public record CurrentUser
{
  public Guid Id { get; set; }
  public Guid SessionId { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public CurrentUser()
  {
  }

  public CurrentUser(SessionModel session) : this(session.User)
  {
    SessionId = session.Id;
  }

  public CurrentUser(UserModel user)
  {
    Id = user.Id;

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;

    if (user.Sessions.Count == 1)
    {
      SessionId = user.Sessions.Single().Id;
    }
  }
}
