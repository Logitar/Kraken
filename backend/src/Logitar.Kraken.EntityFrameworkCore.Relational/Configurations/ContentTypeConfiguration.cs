using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class ContentTypeConfiguration : AggregateConfiguration<ContentTypeEntity>, IEntityTypeConfiguration<ContentTypeEntity>
{
  public override void Configure(EntityTypeBuilder<ContentTypeEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.ContentTypes.Table.Table ?? string.Empty, KrakenDb.ContentTypes.Table.Schema);
    builder.HasKey(x => x.ContentTypeId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.IsInvariant);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.FieldCount);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.ContentTypes)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
