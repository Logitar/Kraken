using Bogus;

namespace Logitar.Kraken.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentLocaleTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "It should construct the correct content locale with field values.")]
  public void Given_FieldValues_When_ctor_Then_Constructed()
  {
    UniqueName uniqueName = new(Content.UniqueNameSettings, _faker.Person.UserName);
    Dictionary<Guid, string> fieldValues = new()
    {
      [Guid.Empty] = $"  {_faker.Person.FullName}  ",
      [Guid.NewGuid()] = _faker.Person.DateOfBirth.ToISOString(),
      [Guid.NewGuid()] = _faker.Person.Gender.ToString(),
      [Guid.NewGuid()] = null!,
      [Guid.NewGuid()] = string.Empty,
      [Guid.NewGuid()] = "   "
    };

    ContentLocale locale = new(uniqueName, fieldValues: fieldValues);

    Assert.Equal(uniqueName, locale.UniqueName);
    Assert.Null(locale.DisplayName);
    Assert.Null(locale.Description);

    Assert.Equal(3, locale.FieldValues.Count);
    foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
    {
      if (!string.IsNullOrWhiteSpace(fieldValue.Value))
      {
        Assert.Equal(fieldValue.Value.Trim(), locale.FieldValues[fieldValue.Key]);
      }
    }
  }

  [Fact(DisplayName = "It should construct the correct content locale without field values.")]
  public void Given_NoFieldValue_When_ctor_Then_Constructed()
  {
    UniqueName uniqueName = new(Content.UniqueNameSettings, _faker.Person.UserName);
    DisplayName displayName = new(_faker.Person.FullName);
    Description description = new(_faker.Lorem.Paragraph());

    ContentLocale locale = new(uniqueName, displayName, description);

    Assert.Equal(uniqueName, locale.UniqueName);
    Assert.Equal(displayName, locale.DisplayName);
    Assert.Equal(description, locale.Description);
    Assert.Empty(locale.FieldValues);
  }
}
