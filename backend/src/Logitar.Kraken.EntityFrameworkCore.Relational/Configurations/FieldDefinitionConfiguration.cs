using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class FieldDefinitionConfiguration : IEntityTypeConfiguration<FieldDefinitionEntity>
{
  public void Configure(EntityTypeBuilder<FieldDefinitionEntity> builder)
  {
    builder.ToTable(KrakenDb.FieldDefinitions.Table.Table ?? string.Empty, KrakenDb.FieldDefinitions.Table.Schema);
    builder.HasKey(x => x.FieldDefinitionId);

    builder.HasIndex(x => new { x.ContentTypeId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.ContentTypeId, x.Order }).IsUnique();
    builder.HasIndex(x => new { x.ContentTypeId, x.UniqueNameNormalized }).IsUnique();

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Placeholder).HasMaxLength(Placeholder.MaximumLength);

    builder.HasOne(x => x.ContentType).WithMany(x => x.Fields)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.FieldType).WithMany(x => x.FieldDefinitions)
      .HasPrincipalKey(x => x.FieldTypeId).HasForeignKey(x => x.FieldTypeId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
