using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class FieldTypeConfiguration : AggregateConfiguration<FieldTypeEntity>, IEntityTypeConfiguration<FieldTypeEntity>
{
  public override void Configure(EntityTypeBuilder<FieldTypeEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.FieldTypes.Table.Table ?? string.Empty, KrakenDb.FieldTypes.Table.Schema);
    builder.HasKey(x => x.FieldTypeId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.DataType);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.DataType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<DataType>());

    builder.HasOne(x => x.Realm).WithMany(x => x.FieldTypes)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
