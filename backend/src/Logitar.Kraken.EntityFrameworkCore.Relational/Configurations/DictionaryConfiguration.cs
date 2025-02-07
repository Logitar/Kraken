using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class DictionaryConfiguration : AggregateConfiguration<DictionaryEntity>, IEntityTypeConfiguration<DictionaryEntity>
{
  public override void Configure(EntityTypeBuilder<DictionaryEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.Dictionaries.Table.Table ?? string.Empty, KrakenDb.Dictionaries.Table.Schema);
    builder.HasKey(x => x.DictionaryId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.EntryCount);

    builder.HasOne(x => x.Realm).WithMany(x => x.Dictionaries)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Language).WithOne(x => x.Dictionary)
      .HasPrincipalKey<LanguageEntity>(x => x.LanguageId).HasForeignKey<DictionaryEntity>(x => x.LanguageId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
