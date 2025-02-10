using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public static class QueryingExtensions
{
  public static IQueryable<T> WhereRealm<T>(this IQueryable<T> query, RealmId? realmId) where T : ISegregatedEntity
  {
    return realmId.HasValue
      ? query.Where(x => x.RealmUid == realmId.Value.ToGuid())
      : query.Where(x => x.RealmId == null);
  }
}
