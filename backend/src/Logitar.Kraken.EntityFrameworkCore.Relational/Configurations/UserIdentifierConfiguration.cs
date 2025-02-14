using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class UserIdentifierConfiguration : IdentifierConfiguration<UserIdentifierEntity>, IEntityTypeConfiguration<UserIdentifierEntity>
{
  public override void Configure(EntityTypeBuilder<UserIdentifierEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.UserIdentifiers.Table.Table ?? string.Empty, KrakenDb.UserIdentifiers.Table.Schema);
    builder.HasKey(x => x.UserIdentifierId);

    builder.HasIndex(x => new { x.UserId, x.Key }).IsUnique();
    builder.HasIndex(x => x.Key);
    builder.HasIndex(x => x.Value);

    builder.HasOne(x => x.Realm).WithMany(x => x.UserIdentifiers)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.User).WithMany(x => x.Identifiers)
      .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
