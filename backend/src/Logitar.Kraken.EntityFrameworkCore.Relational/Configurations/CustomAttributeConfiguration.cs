using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class CustomAttributeConfiguration : IEntityTypeConfiguration<CustomAttributeEntity>
{
  public void Configure(EntityTypeBuilder<CustomAttributeEntity> builder)
  {
    builder.ToTable(KrakenDb.CustomAttributes.Table.Table ?? string.Empty, KrakenDb.CustomAttributes.Table.Schema);
    builder.HasKey(x => x.CustomAttributeId);

    builder.HasIndex(x => new { x.EntityType, x.EntityId });
    builder.HasIndex(x => new { x.EntityType, x.EntityId, x.Key }).IsUnique();
    builder.HasIndex(x => x.Key);
    builder.HasIndex(x => x.ValueShortened);

    builder.Property(x => x.EntityType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Key).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.ValueShortened).HasMaxLength(CustomAttributeEntity.ValueShortenedLength);
  }
}
