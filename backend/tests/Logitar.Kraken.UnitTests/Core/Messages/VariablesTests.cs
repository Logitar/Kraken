using Bogus;
using Logitar.Kraken.Contracts.Messages;

namespace Logitar.Kraken.Core.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class VariablesTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "It should construct the correct default instance.")]
  public void Given_NoArgument_When_ctor_Then_EmptyInstance()
  {
    Variables variables = new();
    Assert.Empty(variables.AsDictionary());
  }

  [Fact(DisplayName = "It should construct the correct instance from capacity.")]
  public void Given_Capacity_When_ctor_Then_EmptyInstance()
  {
    int capacity = 10;
    Variables variables = new(capacity);
    Assert.Empty(variables.AsDictionary());

    Dictionary<string, string> dictionary = GetDictionary(variables);
    Assert.Equal(capacity + 1, dictionary.Capacity);
  }

  [Fact(DisplayName = "It should construct the correct instance from variables.")]
  public void Given_Variables_When_ctor_Then_EmptyInstance()
  {
    string email = _faker.Person.Email;
    string name = _faker.Person.FullName;
    Variable[] pairs =
    [
      new Variable("name", name),
      new Variable("email", email),
      new Variable(" email ", $"  {email}  "),
      new Variable(key: string.Empty, value: Guid.NewGuid().ToString()),
      new Variable(key: "token", value: "    ")
    ];
    Variables variables = new(pairs);

    Dictionary<string, string> dictionary = GetDictionary(variables);
    Assert.Equal(pairs.Length + 2, dictionary.Capacity);
    Assert.Equal(2, dictionary.Count);
    Assert.Contains(dictionary, v => v.Key == "email" && v.Value == email);
    Assert.Contains(dictionary, v => v.Key == "name" && v.Value == name);
  }

  [Fact(DisplayName = "Resolve: it should return the key when the variable was not found.")]
  public void Given_NotFound_When_Resolve_Then_KeyReturned()
  {
    Variables variables = new();
    string key = "token";
    Assert.Equal(key, variables.Resolve(key));
  }

  [Fact(DisplayName = "Resolve: it should return the value when the variable was found.")]
  public void Given_Found_When_Resolve_Then_ValueReturned()
  {
    string key = "token";
    string value = Guid.NewGuid().ToString();
    Variables variables = new([new Variable(key, value)]);
    Assert.Equal(value, variables.Resolve(key));
  }

  private static Dictionary<string, string> GetDictionary(Variables variables)
  {
    FieldInfo? field = variables.GetType().GetField("_variables", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(field);

    Dictionary<string, string>? dictionary = field.GetValue(variables) as Dictionary<string, string>;
    Assert.NotNull(dictionary);

    return dictionary;
  }
}
