using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class ContentConfiguration : AggregateConfiguration<ContentEntity>, IEntityTypeConfiguration<ContentEntity>
{
  public override void Configure(EntityTypeBuilder<ContentEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.Contents.Table.Table ?? string.Empty, KrakenDb.Contents.Table.Schema);
    builder.HasKey(x => x.ContentId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Id);

    builder.HasOne(x => x.Realm).WithMany(x => x.Contents)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.ContentType).WithMany(x => x.Contents)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
