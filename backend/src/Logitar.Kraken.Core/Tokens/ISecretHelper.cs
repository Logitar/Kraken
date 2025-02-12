using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Tokens;

public interface ISecretHelper
{
  Secret Encrypt(string secret, RealmId? realmId = null);
  Secret Generate(RealmId? realmId = null);
}
