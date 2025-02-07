namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public abstract class IdentifierEntity
{
  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string Value { get; protected set; } = string.Empty;

  protected IdentifierEntity()
  {
  }

  protected IdentifierEntity(RealmEntity? realm, string key)
  {
    Realm = realm;
    RealmId = realm?.RealmId;

    Key = key;
  }
}
