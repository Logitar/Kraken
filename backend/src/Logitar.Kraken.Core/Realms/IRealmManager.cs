namespace Logitar.Kraken.Core.Realms;

public interface IRealmManager
{
  Task SaveAsync(Realm realm, CancellationToken cancellationToken = default);
}
