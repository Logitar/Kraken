namespace Logitar.Kraken.Contracts.Users;

public record ChangePasswordPayload
{
  public string? Current { get; set; }
  public string New { get; set; } = string.Empty;

  public ChangePasswordPayload()
  {
  }

  public ChangePasswordPayload(string newPassword, string? currentPassword = null)
  {
    Current = currentPassword;
    New = newPassword;
  }
}
