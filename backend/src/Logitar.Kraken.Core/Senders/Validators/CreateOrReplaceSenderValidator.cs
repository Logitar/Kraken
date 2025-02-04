using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Validators;

internal class CreateOrReplaceSenderValidator : AbstractValidator<CreateOrReplaceSenderPayload>
{
  public CreateOrReplaceSenderValidator()
  {
    //SenderType type = provider.GetSenderType();
    //switch (type)
    //{
    //  case SenderType.Email:
    //    RuleFor(x => x.EmailAddress).NotNull();
    //    When(x => x.EmailAddress != null, () => RuleFor(x => x.EmailAddress!).EmailAddressInput());
    //    RuleFor(x => x.PhoneNumber).Empty();
    //    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    //    break;
    //  case SenderType.Sms:
    //    RuleFor(x => x.EmailAddress).Empty();
    //    RuleFor(x => x.PhoneNumber).NotNull();
    //    When(x => x.PhoneNumber != null, () => RuleFor(x => x.PhoneNumber!).PhoneNumber());
    //    RuleFor(x => x.DisplayName).Empty();
    //    break;
    //  default:
    //    throw new SenderTypeNotSupportedException(type);
    //}

    //When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    //switch (provider)
    //{
    //  case SenderProvider.Mailgun:
    //    RuleFor(x => x.Mailgun).NotNull();
    //    When(x => x.Mailgun != null, () => RuleFor(x => x.Mailgun!).SetValidator(new MailgunSettingsValidator()));
    //    RuleFor(x => x.SendGrid).Null();
    //    RuleFor(x => x.Twilio).Null();
    //    break;
    //  case SenderProvider.SendGrid:
    //    RuleFor(x => x.Mailgun).Null();
    //    RuleFor(x => x.SendGrid).NotNull();
    //    When(x => x.SendGrid != null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
    //    RuleFor(x => x.Twilio).Null();
    //    break;
    //  case SenderProvider.Twilio:
    //    RuleFor(x => x.Mailgun).Null();
    //    RuleFor(x => x.SendGrid).Null();
    //    RuleFor(x => x.Twilio).NotNull();
    //    When(x => x.Twilio != null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()));
    //    break;
    //  default:
    //    throw new SenderProviderNotSupportedException(provider);
    //} // TODO(fpion): implement
  }
}
