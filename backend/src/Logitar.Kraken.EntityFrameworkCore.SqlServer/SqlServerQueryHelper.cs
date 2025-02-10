using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Kraken.EntityFrameworkCore.Relational;

namespace Logitar.Kraken.EntityFrameworkCore.SqlServer;

public class SqlServerQueryHelper : QueryHelper
{
  public override IQueryBuilder From(TableId table) => SqlServerQueryBuilder.From(table);
}
