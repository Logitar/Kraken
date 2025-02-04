using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

public class TwilioSettingsValidator : AbstractValidator<ITwilioSettings>
{
  public TwilioSettingsValidator()
  {
    RuleFor(x => x.AccountSid).NotEmpty();
    RuleFor(x => x.AuthenticationToken).NotEmpty();
  }
}
