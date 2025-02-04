using FluentValidation;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Messages.Validators;

internal class SendMessageValidator : AbstractValidator<SendMessagePayload>
{
  public SendMessageValidator(SenderType senderType)
  {
    RuleFor(x => x.Template).NotEmpty();

    RuleFor(x => x.Recipients).Must(r => r.Any(x => x.Type == RecipientType.To))
      .WithErrorCode("RecipientsValidator")
      .WithMessage($"'{{PropertyName}}' must contain at least one {nameof(RecipientType.To)} recipient.");
    RuleForEach(x => x.Recipients).SetValidator(new RecipientValidator(senderType));

    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());

    RuleForEach(x => x.Variables).SetValidator(new VariableValidator());
  }
}
