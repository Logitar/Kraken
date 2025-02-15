using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Tokens;

public interface ISecretHelper
{
  string Decrypt(Secret secret, RealmId? realmId = null);
  Secret Encrypt(string secret, RealmId? realmId = null);
  Secret Generate(RealmId? realmId = null);
  string Resolve(string? secret = null);
}
