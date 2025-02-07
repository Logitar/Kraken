using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Dictionaries
{
  public static readonly TableId Table = new(Schemas.Localization, nameof(KrakenContext.Dictionaries), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(DictionaryEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(DictionaryEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(DictionaryEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(DictionaryEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(DictionaryEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(DictionaryEntity.Version), Table);

  public static readonly ColumnId DictionaryId = new(nameof(DictionaryEntity.DictionaryId), Table);
  public static readonly ColumnId Entries = new(nameof(DictionaryEntity.Entries), Table);
  public static readonly ColumnId EntryCount = new(nameof(DictionaryEntity.EntryCount), Table);
  public static readonly ColumnId Id = new(nameof(DictionaryEntity.Id), Table);
  public static readonly ColumnId LanguageId = new(nameof(DictionaryEntity.LanguageId), Table);
  public static readonly ColumnId RealmId = new(nameof(DictionaryEntity.RealmId), Table);
}
