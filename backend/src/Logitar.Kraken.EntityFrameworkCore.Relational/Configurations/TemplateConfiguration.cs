using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class TemplateConfiguration : AggregateConfiguration<TemplateEntity>, IEntityTypeConfiguration<TemplateEntity>
{
  public const int ContentTypeMaximumLength = 10; // NOTE(fpion): length of 'text/plain'

  public override void Configure(EntityTypeBuilder<TemplateEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.Templates.Table.Table ?? string.Empty, KrakenDb.Templates.Table.Schema);
    builder.HasKey(x => x.TemplateId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.UniqueKey);
    builder.HasIndex(x => new { x.RealmId, x.UniqueKeyNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.ContentType);

    builder.Property(x => x.UniqueKey).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.UniqueKeyNormalized).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Subject).HasMaxLength(Subject.MaximumLength);
    builder.Property(x => x.ContentType).HasMaxLength(ContentTypeMaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.Templates)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
