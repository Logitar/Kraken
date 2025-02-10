using Logitar.Data;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public interface IQueryHelper
{
  void ApplyTextSearch(IQueryBuilder query, TextSearch search, params ColumnId[] columns);

  IQueryBuilder From(TableId table);
}
