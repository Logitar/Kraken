﻿using Logitar.Kraken.Core;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Configurations;

public abstract class IdentifierConfiguration<T> where T : IdentifierEntity
{
  public virtual void Configure(EntityTypeBuilder<T> builder)
  {
    builder.HasIndex(x => new { x.RealmId, x.Key, x.Value }).IsUnique();
    builder.HasIndex(x => x.Key);
    builder.HasIndex(x => x.Value);

    builder.Property(x => x.Key).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.Value).HasMaxLength(CustomIdentifier.MaximumLength);
  }
}
