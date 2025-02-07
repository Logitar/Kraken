using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedTokenEntity>
{
  public void Configure(EntityTypeBuilder<BlacklistedTokenEntity> builder)
  {
    builder.ToTable(KrakenDb.TokenBlacklist.Table.Table ?? string.Empty, KrakenDb.TokenBlacklist.Table.Schema);
    builder.HasKey(x => x.BlacklistedTokenId);

    builder.HasIndex(x => x.TokenId).IsUnique();
    builder.HasIndex(x => x.ExpiresOn);

    builder.Property(x => x.TokenId).HasMaxLength(byte.MaxValue);
  }
}
