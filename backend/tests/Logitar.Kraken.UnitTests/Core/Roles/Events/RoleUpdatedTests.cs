using Logitar.Kraken.Core.Users;
using Logitar.Kraken.Infrastructure.Converters;

namespace Logitar.Kraken.Core.Roles.Events;

[Trait(Traits.Category, Categories.Unit)]
public class RoleUpdatedTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public RoleUpdatedTests()
  {
    _serializerOptions.Converters.Add(new IdentifierConverter());
  }

  [Fact(DisplayName = "It should be serialized and deserialized correctly.")]
  public void Given_UpdatedEvent_When_Serialize_Then_SerializedCorrectly()
  {
    RoleUpdated updated = new();
    updated.CustomAttributes[new Identifier("UserId")] = UserId.NewId(realmId: null).Value;

    string json = JsonSerializer.Serialize(updated, _serializerOptions);
    RoleUpdated? deserialized = JsonSerializer.Deserialize<RoleUpdated>(json, _serializerOptions);

    Assert.NotNull(deserialized);
    Assert.Equal(updated.CustomAttributes, deserialized.CustomAttributes);
  }
}
