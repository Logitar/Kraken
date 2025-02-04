using FluentValidation;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders;

namespace Logitar.Kraken.Core.Messages.Validators;

internal class RecipientValidator : AbstractValidator<RecipientPayload>
{
  public RecipientValidator(SenderType senderType)
  {
    RuleFor(x => x.Type).IsInEnum();

    When(x => !string.IsNullOrWhiteSpace(x.Address), () => RuleFor(x => x.Address!).EmailAddressInput());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () => RuleFor(x => x.PhoneNumber!).PhoneNumber());

    switch (senderType)
    {
      case SenderType.Email:
        RuleFor(x => x.PhoneNumber).Empty();
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Address) || x.UserId.HasValue)
          .WithErrorCode(nameof(RecipientValidator))
          .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.Address)}, {nameof(RecipientPayload.UserId)}.");
        break;
      case SenderType.Phone:
        RuleFor(x => x.Address).Empty();
        RuleFor(x => x.DisplayName).Empty();
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.PhoneNumber) || x.UserId.HasValue)
          .WithErrorCode(nameof(RecipientValidator))
          .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.PhoneNumber)}, {nameof(RecipientPayload.UserId)}.");
        break;
      default:
        throw new SenderTypeNotSupportedException(senderType);
    }
  }
}
