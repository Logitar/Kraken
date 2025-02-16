using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class RealmConfiguration : AggregateConfiguration<RealmEntity>, IEntityTypeConfiguration<RealmEntity>
{
  public override void Configure(EntityTypeBuilder<RealmEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.Realms.Table.Table ?? string.Empty, KrakenDb.Realms.Table.Schema);
    builder.HasKey(x => x.RealmId);

    builder.HasIndex(x => x.UniqueSlug);
    builder.HasIndex(x => x.UniqueSlugNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueSlug).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.UniqueSlugNormalized).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Secret).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.AllowedUniqueNameCharacters).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordHashingStrategy).HasMaxLength(byte.MaxValue);
  }
}
