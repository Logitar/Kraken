using Logitar.Kraken.Infrastructure.Converters;

namespace Logitar.Kraken.Core.Realms.Events;

[Trait(Traits.Category, Categories.Unit)]
public class RealmUpdatedTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public RealmUpdatedTests()
  {
    _serializerOptions.Converters.Add(new IdentifierConverter());
  }

  [Fact(DisplayName = "It should be serialized and deserialized correctly.")]
  public void Given_UpdatedEvent_When_Serialize_Then_SerializedCorrectly()
  {
    RealmUpdated updated = new();
    updated.CustomAttributes[new Identifier("RealmId")] = RealmId.NewId().Value;

    var json = JsonSerializer.Serialize(updated, _serializerOptions);
    var deserialized = JsonSerializer.Deserialize<RealmUpdated>(json, _serializerOptions);

    Assert.NotNull(deserialized);
    Assert.Equal(updated.CustomAttributes, deserialized.CustomAttributes);
  }
}
