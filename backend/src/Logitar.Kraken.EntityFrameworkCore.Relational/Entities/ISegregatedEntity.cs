namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public interface ISegregatedEntity
{
  RealmEntity? Realm { get; }
  int? RealmId { get; }
  Guid? RealmUid { get; }
}
