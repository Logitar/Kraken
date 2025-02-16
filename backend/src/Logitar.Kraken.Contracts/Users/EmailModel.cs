namespace Logitar.Kraken.Contracts.Users;

public record EmailModel : ContactModel, IEmail
{
  public string Address { get; set; }

  public EmailModel() : this(string.Empty)
  {
  }

  public EmailModel(IEmail email) : this(email.Address)
  {
    IsVerified = email.IsVerified;
  }

  public EmailModel(string address)
  {
    Address = address;
  }
}
