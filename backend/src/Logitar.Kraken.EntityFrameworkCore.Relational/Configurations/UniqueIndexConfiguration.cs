using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class UniqueIndexConfiguration : IEntityTypeConfiguration<UniqueIndexEntity>
{
  public void Configure(EntityTypeBuilder<UniqueIndexEntity> builder)
  {
    builder.ToTable(KrakenDb.UniqueIndex.Table.Table ?? string.Empty, KrakenDb.UniqueIndex.Table.Schema);
    builder.HasKey(x => x.UniqueIndexId);

    builder.HasIndex(x => x.RealmId);
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.RealmSlug);
    builder.HasIndex(x => x.ContentTypeId);
    builder.HasIndex(x => x.ContentTypeUid);
    builder.HasIndex(x => x.ContentTypeName);
    builder.HasIndex(x => x.LanguageId);
    builder.HasIndex(x => x.LanguageUid);
    builder.HasIndex(x => x.LanguageCode);
    builder.HasIndex(x => x.LanguageIsDefault);
    builder.HasIndex(x => x.FieldTypeId);
    builder.HasIndex(x => x.FieldTypeUid);
    builder.HasIndex(x => x.FieldTypeName);
    builder.HasIndex(x => x.FieldDefinitionId);
    builder.HasIndex(x => x.FieldDefinitionUid);
    builder.HasIndex(x => x.FieldDefinitionName);
    builder.HasIndex(x => x.Revision);
    builder.HasIndex(x => x.Status);
    builder.HasIndex(x => x.Value);
    builder.HasIndex(x => x.ValueNormalized);
    builder.HasIndex(x => x.Key);
    builder.HasIndex(x => x.ContentId);
    builder.HasIndex(x => x.ContentUid);
    builder.HasIndex(x => x.ContentLocaleId);
    builder.HasIndex(x => x.ContentLocaleName);
    builder.HasIndex(x => new { x.FieldDefinitionId, x.LanguageId, x.Status, x.ValueNormalized }).IsUnique();

    builder.Property(x => x.ContentTypeName).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.LanguageCode).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.FieldTypeName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.FieldDefinitionName).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(10).HasConversion(new EnumToStringConverter<ContentStatus>()); // NOTE(fpion): the longest status name, Published, is only 9 characters.
    builder.Property(x => x.Value).HasMaxLength(UniqueIndexEntity.MaximumLength);
    builder.Property(x => x.ValueNormalized).HasMaxLength(UniqueIndexEntity.MaximumLength);
    builder.Property(x => x.Key).HasMaxLength(UniqueIndexEntity.MaximumLength + 22 + 1); // NOTE(fpion): 22 base64 characters in a Guid and 1 separator character.
    builder.Property(x => x.ContentLocaleName).HasMaxLength(UniqueName.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.UniqueIndex)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.ContentType).WithMany(x => x.UniqueIndex)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Language).WithMany(x => x.UniqueIndex)
      .HasPrincipalKey(x => x.LanguageId).HasForeignKey(x => x.LanguageId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.FieldType).WithMany(x => x.UniqueIndex)
      .HasPrincipalKey(x => x.FieldTypeId).HasForeignKey(x => x.FieldTypeId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.FieldDefinition).WithMany(x => x.UniqueIndex)
      .HasPrincipalKey(x => x.FieldDefinitionId).HasForeignKey(x => x.FieldDefinitionId)
      .OnDelete(DeleteBehavior.NoAction);
    builder.HasOne(x => x.Content).WithMany(x => x.UniqueIndex)
      .HasPrincipalKey(x => x.ContentId).HasForeignKey(x => x.ContentId)
      .OnDelete(DeleteBehavior.NoAction);
    builder.HasOne(x => x.ContentLocale).WithMany(x => x.UniqueIndex)
      .HasPrincipalKey(x => x.ContentLocaleId).HasForeignKey(x => x.ContentLocaleId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
