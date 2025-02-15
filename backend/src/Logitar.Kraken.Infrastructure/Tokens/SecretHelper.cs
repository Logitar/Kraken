using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Infrastructure.Tokens;

internal class SecretHelper : ISecretHelper
{
  private readonly IPasswordManager _passwordManager;

  public SecretHelper(IPasswordManager passwordManager)
  {
    _passwordManager = passwordManager;
  }

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
    _ = _passwordManager.Generate(Secret.MinimumLength, out string secret);
    return Encrypt(secret, realmId);
  }
}
