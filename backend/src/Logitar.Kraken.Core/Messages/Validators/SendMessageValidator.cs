using FluentValidation;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Messages.Validators;

internal class SendMessageValidator : AbstractValidator<SendMessagePayload>
{
  public SendMessageValidator(SenderType senderType)
  {
    When(x => !string.IsNullOrWhiteSpace(x.Sender), () => RuleFor(x => x.Sender!).Must(BeAValidSender)
      .WithErrorCode("SenderValidator")
      .WithMessage($"'{{PropertyName}}' must be empty, '{nameof(SenderType.Email)}', '{nameof(SenderType.Phone)}', or the identifier of a sender."));
    RuleFor(x => x.Template).NotEmpty();

    RuleFor(x => x.Recipients).Must(recipient => recipient.Any(x => x.Type == RecipientType.To))
      .WithErrorCode("RecipientsValidator")
      .WithMessage($"'{{PropertyName}}' must contain at least one {nameof(RecipientType.To)} recipient.");
    RuleForEach(x => x.Recipients).SetValidator(new RecipientValidator(senderType));

    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());

    RuleForEach(x => x.Variables).SetValidator(new VariableValidator());
  }

  private static bool BeAValidSender(string sender) => Guid.TryParse(sender, out _) || Enum.TryParse<SenderType>(sender, out _);
}
