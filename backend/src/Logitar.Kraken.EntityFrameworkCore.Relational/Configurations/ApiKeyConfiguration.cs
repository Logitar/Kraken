using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class ApiKeyConfiguration : AggregateConfiguration<ApiKeyEntity>, IEntityTypeConfiguration<ApiKeyEntity>
{
  public override void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.ApiKeys.Table.Table ?? string.Empty, KrakenDb.ApiKeys.Table.Schema);
    builder.HasKey(x => x.ApiKeyId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.ExpiresOn);
    builder.HasIndex(x => x.AuthenticatedOn);

    builder.Property(x => x.SecretHash).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Name).HasMaxLength(DisplayName.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.ApiKeys)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasMany(x => x.Roles).WithMany(x => x.ApiKeys).UsingEntity<ApiKeyRoleEntity>(joinBuilder =>
    {
      joinBuilder.ToTable(KrakenDb.ApiKeyRoles.Table.Table ?? string.Empty, KrakenDb.ApiKeyRoles.Table.Schema);
      joinBuilder.HasKey(x => new { x.ApiKeyId, x.RoleId });
    });
  }
}
