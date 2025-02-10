using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public sealed class MessageConfiguration : AggregateConfiguration<MessageEntity>, IEntityTypeConfiguration<MessageEntity>
{
  public override void Configure(EntityTypeBuilder<MessageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenDb.Messages.Table.Table ?? string.Empty, KrakenDb.Messages.Table.Schema);
    builder.HasKey(x => x.MessageId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.RecipientCount);
    builder.HasIndex(x => x.IsDemo);
    builder.HasIndex(x => x.Status);

    builder.Property(x => x.Subject).HasMaxLength(Subject.MaximumLength);
    builder.Property(x => x.BodyType).HasMaxLength(TemplateConfiguration.ContentTypeMaximumLength);
    builder.Property(x => x.SenderAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.SenderPhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.SenderDisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.SenderProvider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderProvider>());
    builder.Property(x => x.TemplateUniqueKey).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.TemplateDisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Locale).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<MessageStatus>());

    builder.HasOne(x => x.Realm).WithMany(x => x.Messages)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Sender).WithMany(x => x.Messages)
      .HasPrincipalKey(x => x.SenderId).HasForeignKey(x => x.SenderId)
      .OnDelete(DeleteBehavior.SetNull);
    builder.HasOne(x => x.Template).WithMany(x => x.Messages)
      .HasPrincipalKey(x => x.TemplateId).HasForeignKey(x => x.TemplateId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
