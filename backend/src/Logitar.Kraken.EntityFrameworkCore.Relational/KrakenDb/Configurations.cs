﻿using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Configurations
{
  public static readonly TableId Table = new(Schemas.Kraken, nameof(KrakenContext.Configurations), alias: null);

  public static readonly ColumnId ConfigurationId = new(nameof(ConfigurationEntity.ConfigurationId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(ConfigurationEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ConfigurationEntity.CreatedOn), Table);
  public static readonly ColumnId Key = new(nameof(ConfigurationEntity.Key), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ConfigurationEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ConfigurationEntity.UpdatedOn), Table);
  public static readonly ColumnId Value = new(nameof(ConfigurationEntity.Value), Table);
  public static readonly ColumnId Version = new(nameof(ConfigurationEntity.Version), Table);
}
