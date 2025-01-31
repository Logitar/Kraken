using FluentValidation;
using Logitar.Kraken.Contracts.Logging;
using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings.Validators;

internal class LoggingSettingsValidator : AbstractValidator<ILoggingSettings>
{
  public LoggingSettingsValidator()
  {
    RuleFor(x => x.Extent).IsInEnum();
    When(x => x.OnlyErrors, () => RuleFor(x => x.Extent).NotEqual(LoggingExtent.None)
      .WithErrorCode(nameof(LoggingSettingsValidator))
      .WithMessage(x => $"The logging extent must not be set to '{nameof(LoggingExtent.None)}' when '{nameof(x.OnlyErrors)}' is true."));
  }
}
