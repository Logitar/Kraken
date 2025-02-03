﻿using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Sessions;

[Trait(Traits.Category, Categories.Unit)]
public class SessionIdTests
{
  [Theory(DisplayName = "ctor: it should construct the correct ID from a stream ID.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_StreamId_When_ctor_Then_CorrectIdConstructed(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();
    StreamId streamId = IdHelper.Encode(realmId, entityId);

    SessionId id = new(streamId);

    Assert.Equal(realmId, id.RealmId);
    Assert.Equal(entityId, id.EntityId);
  }

  [Theory(DisplayName = "ctor: it should construct the correct ID from a realm ID and an entity ID.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_RealmAndEntityId_When_ctor_Then_CorrectIdConstructed(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();

    SessionId id = new(realmId, entityId);

    Assert.Equal(realmId, id.RealmId);
    Assert.Equal(entityId, id.EntityId);
  }

  [Fact(DisplayName = "Equals: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_Equals_Then_FalseReturned()
  {
    SessionId id1 = new(RealmId.NewId(), Guid.NewGuid());
    SessionId id2 = new(realmId: null, id1.EntityId);
    Assert.False(id1.Equals(id2));
  }

  [Theory(DisplayName = "Equals: it should return false when the object do not have the same types.")]
  [InlineData(null)]
  [InlineData(123)]
  public void Given_DifferentTypes_When_Equals_Then_FalseReturned(object? value)
  {
    SessionId id = SessionId.NewId(realmId: null);
    Assert.False(id.Equals(value));
  }

  [Fact(DisplayName = "Equals: it should return true when the IDs are the same.")]
  public void Given_SameIds_When_Equals_Then_TrueReturned()
  {
    SessionId id1 = new(RealmId.NewId(), Guid.NewGuid());
    SessionId id2 = new(id1.StreamId);
    Assert.True(id1.Equals(id1));
    Assert.True(id1.Equals(id2));
  }

  [Fact(DisplayName = "EqualOperator: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_EqualOperator_Then_FalseReturned()
  {
    SessionId id1 = new(RealmId.NewId(), Guid.NewGuid());
    SessionId id2 = new(realmId: null, id1.EntityId);
    Assert.False(id1 == id2);
  }

  [Fact(DisplayName = "EqualOperator: it should return true when the IDs are the same.")]
  public void Given_SameIds_When_EqualOperator_Then_TrueReturned()
  {
    SessionId id1 = new(RealmId.NewId(), Guid.NewGuid());
    SessionId id2 = new(id1.StreamId);
    Assert.True(id1 == id2);
  }

  [Theory(DisplayName = "NewId: it should generate a new random ID with or without a realm ID.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_RealmId_When_NewId_Then_NewRandomIdGenerated(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(realmIdValue);

    SessionId id = SessionId.NewId(realmId);

    Assert.Equal(realmId, id.RealmId);
    Assert.NotEqual(Guid.Empty, id.EntityId);
  }

  [Theory(DisplayName = "GetHashCode: it should return the correct hash code.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_Id_When_GetHashCode_Then_CorrectHashCodeReturned(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();

    SessionId id = new(realmId, entityId);

    Assert.Equal(id.Value.GetHashCode(), id.GetHashCode());
  }

  [Fact(DisplayName = "NotEqualOperator: it should return false when the IDs are the same.")]
  public void Given_SameIds_When_NotEqualOperator_Then_TrueReturned()
  {
    SessionId id1 = new(RealmId.NewId(), Guid.NewGuid());
    SessionId id2 = new(id1.StreamId);
    Assert.False(id1 != id2);
  }

  [Fact(DisplayName = "NotEqualOperator: it should return true when the IDs are different.")]
  public void Given_DifferentIds_When_NotEqualOperator_Then_TrueReturned()
  {
    SessionId id1 = new(RealmId.NewId(), Guid.NewGuid());
    SessionId id2 = new(realmId: null, id1.EntityId);
    Assert.True(id1 != id2);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_Id_When_ToString_Then_CorrectStringReturned(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();

    SessionId id = new(realmId, entityId);

    Assert.Equal(id.Value, id.ToString());
  }
}
