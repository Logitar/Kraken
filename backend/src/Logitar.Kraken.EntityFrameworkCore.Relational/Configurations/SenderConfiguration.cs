using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class SenderConfiguration : AggregateConfiguration<SenderEntity>, IEntityTypeConfiguration<SenderEntity>
{
  public override void Configure(EntityTypeBuilder<SenderEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.Senders.Table.Table ?? string.Empty, KrakenDb.Senders.Table.Schema);
    builder.HasKey(x => x.SenderId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => new { x.RealmId, x.Type, x.IsDefault });
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.PhoneNumber);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Provider);

    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderType>());
    builder.Property(x => x.EmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Provider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderProvider>());

    builder.HasOne(x => x.Realm).WithMany(x => x.Senders)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
