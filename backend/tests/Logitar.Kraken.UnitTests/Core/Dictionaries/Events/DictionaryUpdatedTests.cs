using Logitar.Kraken.Infrastructure.Converters;

namespace Logitar.Kraken.Core.Dictionaries.Events;

[Trait(Traits.Category, Categories.Unit)]
public class DictionaryUpdatedTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public DictionaryUpdatedTests()
  {
    _serializerOptions.Converters.Add(new IdentifierConverter());
  }

  [Fact(DisplayName = "It should be serialized and deserialized correctly.")]
  public void Given_UpdatedEvent_When_Serialize_Then_SerializedCorrectly()
  {
    DictionaryUpdated updated = new();
    updated.Entries[new Identifier("Red")] = "Rouge";

    string json = JsonSerializer.Serialize(updated, _serializerOptions);
    DictionaryUpdated? deserialized = JsonSerializer.Deserialize<DictionaryUpdated>(json, _serializerOptions);

    Assert.NotNull(deserialized);
    Assert.Equal(updated.Entries, deserialized.Entries);
  }
}
