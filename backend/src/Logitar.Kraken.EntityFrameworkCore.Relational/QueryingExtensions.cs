using Logitar.Data;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder builder, ColumnId column, IEnumerable<Guid> ids)
  {
    if (!ids.Any())
    {
      return builder;
    }
    object[] values = ids.Distinct().Select(id => (object)id).ToArray();
    return builder.Where(column, Operators.IsIn(values));
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    return query.ApplyPaging(payload.Skip, payload.Limit);
  }
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int skip, int limit)
  {
    if (skip > 0)
    {
      query = query.Skip(skip);
    }
    if (limit > 0)
    {
      query = query.Take(limit);
    }
    return query;
  }

  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQueryBuilder query) where T : class
  {
    return entities.FromQuery(query.Build());
  }
  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQuery query) where T : class
  {
    return entities.FromSqlRaw(query.Text, query.Parameters.ToArray());
  }

  public static IQueryBuilder WhereRealm(this IQueryBuilder query, ColumnId column, RealmId? realmId)
  {
    return realmId.HasValue
      ? query.Where(column, Operators.IsEqualTo(realmId.Value.ToGuid()))
      : query.Where(column, Operators.IsNull());
  }
  public static IQueryable<T> WhereRealm<T>(this IQueryable<T> query, RealmId? realmId) where T : ISegregatedEntity
  {
    return realmId.HasValue
      ? query.Where(x => x.RealmUid == realmId.Value.ToGuid())
      : query.Where(x => x.RealmId == null);
  }
}
