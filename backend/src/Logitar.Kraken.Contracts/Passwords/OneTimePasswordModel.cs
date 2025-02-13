using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Contracts.Passwords;

public class OneTimePasswordModel : AggregateModel
{
  public string? Password { get; set; }

  public DateTime? ExpiresOn { get; set; }
  public int? MaximumAttempts { get; set; }

  public int AttemptCount { get; set; }
  public bool HasValidationSucceeded { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];

  public RealmModel? Realm { get; set; }
  public UserModel? User { get; set; }
}
