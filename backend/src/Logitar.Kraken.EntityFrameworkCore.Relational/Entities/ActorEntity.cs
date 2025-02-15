﻿using Logitar.Kraken.Contracts.Actors;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class ActorEntity
{
  public int ActorId { get; private set; }
  public Guid Id { get; private set; }
  public string Key { get; private set; } = string.Empty;

  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  private ActorEntity()
  {
  }

  public override bool Equals(object? obj) => obj is ActorEntity actor && actor.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    StringBuilder actor = new();
    actor.Append(DisplayName);
    if (EmailAddress != null)
    {
      actor.Append(" <").Append(EmailAddress).Append('>');
    }
    actor.Append(" (").Append(Type).Append(".Id=").Append(Id).Append(')');
    return actor.ToString();
  }
}
