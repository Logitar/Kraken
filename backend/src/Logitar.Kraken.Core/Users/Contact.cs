using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core.Users;

public abstract record Contact : IContact
{
  public bool IsVerified { get; }

  protected Contact(bool isVerified)
  {
    IsVerified = isVerified;
  }
}
