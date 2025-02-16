using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Web.Models.Account;

public record ChangeAccountPassword
{
  public string Current { get; set; } = string.Empty;
  public string New { get; set; } = string.Empty;

  public ChangeAccountPassword()
  {
  }

  public ChangeAccountPassword(string currentPassword, string newPassword)
  {
    Current = currentPassword;
    New = newPassword;
  }

  public ChangePasswordPayload ToChangePasswordPayload() => new(New, Current);
}
