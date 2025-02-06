using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class SelectOptionTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    SelectOptionModel model = new()
    {
      Text = "  Rouge  ",
      IsDisabled = true,
      Label = " Rojo ",
      Value = "red"
    };
    SelectOption option = new(model);
    Assert.Equal(model.Text.Trim(), option.Text);
    Assert.Equal(model.IsDisabled, option.IsDisabled);
    Assert.Equal(model.Label.Trim(), option.Label);
    Assert.Equal(model.Value.Trim(), option.Value);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    string text = "  Rouge  ";
    bool isDisabled = true;
    string label = " Rojo ";
    string value = "red";
    SelectOption option = new(text, isDisabled, label, value);
    Assert.Equal(text.Trim(), option.Text);
    Assert.Equal(isDisabled, option.IsDisabled);
    Assert.Equal(label.Trim(), option.Label);
    Assert.Equal(value.Trim(), option.Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new SelectOption(text: string.Empty, isDisabled: false, label: string.Empty, value: "    "));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Text");
  }
}
