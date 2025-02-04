using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Validators;

internal class UpdateSenderValidator : AbstractValidator<UpdateSenderPayload>
{
  public UpdateSenderValidator()
  {
    //SenderType type = provider.GetSenderType();
    //switch (type)
    //{
    //  case SenderType.Email:
    //    When(x => x.EmailAddress != null, () => RuleFor(x => x.EmailAddress!).EmailAddressInput());
    //    RuleFor(x => x.PhoneNumber).Empty();
    //    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    //    break;
    //  case SenderType.Sms:
    //    RuleFor(x => x.EmailAddress).Empty();
    //    When(x => x.PhoneNumber != null, () => RuleFor(x => x.PhoneNumber!).PhoneNumber());
    //    RuleFor(x => x.DisplayName).Empty();
    //    break;
    //  default:
    //    throw new SenderTypeNotSupportedException(type);
    //}

    //When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    //switch (provider)
    //{
    //  case SenderProvider.Mailgun:
    //    RuleFor(x => x.SendGrid).Null();
    //    RuleFor(x => x.Twilio).Null();
    //    break;
    //  case SenderProvider.SendGrid:
    //    RuleFor(x => x.Mailgun).Null();
    //    RuleFor(x => x.Twilio).Null();
    //    break;
    //  case SenderProvider.Twilio:
    //    RuleFor(x => x.Mailgun).Null();
    //    RuleFor(x => x.SendGrid).Null();
    //    break;
    //  default:
    //    throw new SenderProviderNotSupportedException(provider);
    //} // TODO(fpion): implement
  }
}
