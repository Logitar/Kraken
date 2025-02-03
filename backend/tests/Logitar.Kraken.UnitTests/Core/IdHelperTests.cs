using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core;

[Trait(Traits.Category, Categories.Unit)]
public class IdHelperTests
{
  [Theory(DisplayName = "Decode: it shoud decode and decode the correct stream ID.")]
  [InlineData(null)]
  [InlineData("RealmId")]
  public void Given_StreamId_When_DecodeEncode_Then_DecodedEncodedCorrectly(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(realmIdValue);
    Guid entityId = Guid.NewGuid();

    StreamId streamId = IdHelper.Encode(realmId, entityId);
    if (realmIdValue == null)
    {
      Assert.Equal(Convert.ToBase64String(entityId­.ToByteArray()).ToUriSafeBase64(), streamId.Value);
    }
    else
    {
      string[] values = streamId.Value.Split(':');
      Assert.Equal(2, values.Length);
      Assert.Equal(realmIdValue, values[0]);
      Assert.Equal(Convert.ToBase64String(entityId­.ToByteArray()).ToUriSafeBase64(), values[1]);
    }

    Tuple<RealmId?, Guid> decoded = IdHelper.Decode(streamId);
    Assert.Equal(realmId, decoded.Item1);
    Assert.Equal(entityId, decoded.Item2);
  }
}
