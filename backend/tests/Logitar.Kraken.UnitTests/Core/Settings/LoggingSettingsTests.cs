using FluentValidation;
using Logitar.Kraken.Contracts.Logging;
using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class LoggingSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    LoggingSettingsModel model = new()
    {
      Extent = LoggingExtent.ActivityOnly,
      OnlyErrors = true
    };

    LoggingSettings settings = new(model);

    Assert.Equal(model.Extent, settings.Extent);
    Assert.Equal(model.OnlyErrors, settings.OnlyErrors);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    LoggingExtent extent = LoggingExtent.ActivityOnly;

    LoggingSettings settings = new();
    Assert.Equal(extent, settings.Extent);
    Assert.False(settings.OnlyErrors);

    settings = new(extent, onlyErrors: true);

    Assert.Equal(extent, settings.Extent);
    Assert.True(settings.OnlyErrors);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new LoggingSettings(LoggingExtent.None, onlyErrors: true));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LoggingSettingsValidator" && e.PropertyName == "Extent");
  }
}
