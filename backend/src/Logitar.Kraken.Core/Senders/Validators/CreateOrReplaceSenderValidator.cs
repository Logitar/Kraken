using FluentValidation;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders.Settings;

namespace Logitar.Kraken.Core.Senders.Validators;

internal class CreateOrReplaceSenderValidator : AbstractValidator<CreateOrReplaceSenderPayload>
{
  public CreateOrReplaceSenderValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.EmailAddress), () => RuleFor(x => x.EmailAddress!).EmailAddressInput());
    When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () => RuleFor(x => x.EmailAddress!).PhoneNumber());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x).Must(x => GetSenderProviders(x).Count == 1)
      .WithErrorCode("CreateOrReplaceSenderValidator")
      .WithMessage(x => $"Exactly one of the following must be specified: {string.Join(", ", nameof(x.Mailgun), nameof(x.SendGrid), nameof(x.Twilio))}.");
    When(x => x.Mailgun != null, () => RuleFor(x => x.Mailgun!).SetValidator(new MailgunSettingsValidator()));
    When(x => x.SendGrid != null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
    When(x => x.Twilio != null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()));
  }

  private static IReadOnlyCollection<SenderProvider> GetSenderProviders(CreateOrReplaceSenderPayload payload)
  {
    List<SenderProvider> dataTypes = new(capacity: 3);

    if (payload.Mailgun != null)
    {
      dataTypes.Add(SenderProvider.Mailgun);
    }
    if (payload.SendGrid != null)
    {
      dataTypes.Add(SenderProvider.SendGrid);
    }
    if (payload.Twilio != null)
    {
      dataTypes.Add(SenderProvider.Twilio);
    }

    return dataTypes.AsReadOnly();
  }
}
