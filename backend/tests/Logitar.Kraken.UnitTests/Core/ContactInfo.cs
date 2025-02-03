using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core;

internal record ContactInfo
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = string.Empty;
  public List<EmailModel> Emails { get; set; } = [];
}
