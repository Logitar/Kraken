using Logitar.EventSourcing;

namespace Logitar.Kraken.Core.Roles;

public readonly struct RoleId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RoleId(Guid value)
  {
    StreamId = new(value);
  }
  public RoleId(string value)
  {
    StreamId = new(value);
  }
  public RoleId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static RoleId NewId() => new(StreamId.NewId());

  public Guid ToGuid() => StreamId.ToGuid();

  public static bool operator ==(RoleId left, RoleId right) => left.Equals(right);
  public static bool operator !=(RoleId left, RoleId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RoleId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
