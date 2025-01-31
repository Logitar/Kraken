﻿using Logitar.EventSourcing;

namespace Logitar.Kraken.Core.Users;

public readonly struct UserId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public UserId(Guid value)
  {
    StreamId = new(value);
  }
  public UserId(string value)
  {
    StreamId = new(value);
  }
  public UserId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static UserId NewId() => new(StreamId.NewId());

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is UserId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
