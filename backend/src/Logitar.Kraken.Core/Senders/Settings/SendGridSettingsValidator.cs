using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

public class SendGridSettingsValidator : AbstractValidator<ISendGridSettings>
{
  public SendGridSettingsValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty();
  }
}
