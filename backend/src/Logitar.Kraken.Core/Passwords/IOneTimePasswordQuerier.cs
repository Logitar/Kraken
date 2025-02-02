using Logitar.Kraken.Contracts.Passwords;

namespace Logitar.Kraken.Core.Passwords;

public interface IOneTimePasswordQuerier
{
  Task<OneTimePasswordModel> ReadAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);
  Task<OneTimePasswordModel?> ReadAsync(OneTimePasswordId id, CancellationToken cancellationToken = default);
  Task<OneTimePasswordModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
