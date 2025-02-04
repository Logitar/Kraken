using FluentValidation;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders.Settings;

namespace Logitar.Kraken.Core.Senders.Validators;

internal class UpdateSenderValidator : AbstractValidator<UpdateSenderPayload>
{
  public UpdateSenderValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.EmailAddress), () => RuleFor(x => x.EmailAddress!).EmailAddressInput());
    When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () => RuleFor(x => x.EmailAddress!).PhoneNumber());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Mailgun != null, () => RuleFor(x => x.Mailgun!).SetValidator(new MailgunSettingsValidator()));
    When(x => x.SendGrid != null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
    When(x => x.Twilio != null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()));
  }
}
