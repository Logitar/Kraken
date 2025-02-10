using Logitar.Data;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public interface IQueryHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder query, TextSearch search, params ColumnId[] columns);

  IQueryBuilder From(TableId table);
}
