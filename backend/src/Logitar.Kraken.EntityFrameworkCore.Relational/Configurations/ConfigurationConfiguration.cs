using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class ConfigurationConfiguration : IEntityTypeConfiguration<ConfigurationEntity>
{
  public void Configure(EntityTypeBuilder<ConfigurationEntity> builder)
  {
    builder.ToTable(KrakenDb.Configurations.Table.Table ?? string.Empty, KrakenDb.Configurations.Table.Schema);
    builder.HasKey(x => x.ConfigurationId);

    builder.HasIndex(x => x.Key).IsUnique();
    builder.HasIndex(x => x.Value);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.Key).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.Value).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);
  }
}
