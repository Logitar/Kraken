using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Contracts.Sessions;

public class SessionModel : AggregateModel
{
  public bool IsPersistent { get; set; }
  public string? RefreshToken { get; set; }

  public bool IsActive { get; set; }
  public ActorModel? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];

  public UserModel User { get; set; } = new();
}
