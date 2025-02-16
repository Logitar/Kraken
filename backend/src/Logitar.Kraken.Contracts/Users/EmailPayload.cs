namespace Logitar.Kraken.Contracts.Users;

public record EmailPayload : ContactPayload, IEmail
{
  public string Address { get; set; } = string.Empty;

  public EmailPayload() : base()
  {
  }

  public EmailPayload(IEmail email) : this(email.Address, email.IsVerified)
  {
  }

  public EmailPayload(string address, bool isVerified) : base(isVerified)
  {
    Address = address;
  }
}
