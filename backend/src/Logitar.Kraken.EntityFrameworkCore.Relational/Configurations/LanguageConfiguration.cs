﻿using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class LanguageConfiguration : AggregateConfiguration<LanguageEntity>, IEntityTypeConfiguration<LanguageEntity>
{
  public override void Configure(EntityTypeBuilder<LanguageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.Languages.Table.Table ?? string.Empty, KrakenDb.Languages.Table.Schema);
    builder.HasKey(x => x.LanguageId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.IsDefault);
    builder.HasIndex(x => x.LCID);
    builder.HasIndex(x => x.Code);
    builder.HasIndex(x => new { x.RealmId, x.CodeNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.EnglishName);
    builder.HasIndex(x => x.NativeName);

    builder.Property(x => x.Code).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.CodeNormalized).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.EnglishName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.NativeName).HasMaxLength(DisplayName.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.Languages)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
