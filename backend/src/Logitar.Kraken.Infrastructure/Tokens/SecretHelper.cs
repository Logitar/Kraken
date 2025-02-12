using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Tokens;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Infrastructure.Tokens;

internal class SecretHelper : ISecretHelper
{
  public Secret Encrypt(string secret, RealmId? realmId)
  {
    secret = secret.Remove(" ");
    if (secret.Length < Secret.MinimumLength || secret.Length > Secret.MaximumLength)
    {
      throw new ArgumentException($"The secret must be between {Secret.MinimumLength} and {Secret.MaximumLength} characters long (inclusive), excluding any spaces.");
    }

    return new Secret(secret); // TODO(fpion): encrypt
  }

  public Secret Generate(RealmId? realmId)
  {
    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    return Encrypt(secret, realmId);
  }
}
