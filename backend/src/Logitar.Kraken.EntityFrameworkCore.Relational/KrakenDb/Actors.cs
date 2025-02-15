using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Actors
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.Actors), alias: null);

  public static readonly ColumnId ActorId = new(nameof(ActorEntity.ActorId), Table);
  public static readonly ColumnId DisplayName = new(nameof(ActorEntity.DisplayName), Table);
  public static readonly ColumnId EmailAddress = new(nameof(ActorEntity.EmailAddress), Table);
  public static readonly ColumnId Id = new(nameof(ActorEntity.Id), Table);
  public static readonly ColumnId IsDeleted = new(nameof(ActorEntity.IsDeleted), Table);
  public static readonly ColumnId Key = new(nameof(ActorEntity.Key), Table);
  public static readonly ColumnId PictureUrl = new(nameof(ActorEntity.PictureUrl), Table);
  public static readonly ColumnId Type = new(nameof(ActorEntity.Type), Table);
}
