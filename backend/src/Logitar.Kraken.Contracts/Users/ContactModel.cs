using Logitar.Kraken.Contracts.Actors;

namespace Logitar.Kraken.Contracts.Users;

public abstract record ContactModel
{
  public bool IsVerified { get; set; }
  public ActorModel? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
}
