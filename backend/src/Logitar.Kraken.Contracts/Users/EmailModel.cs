namespace Logitar.Kraken.Contracts.Users;

public record EmailModel : ContactModel, IEmail
{
  public string Address { get; set; }

  public EmailModel() : this(string.Empty)
  {
  }

  public EmailModel(string address)
  {
    Address = address;
  }
}
