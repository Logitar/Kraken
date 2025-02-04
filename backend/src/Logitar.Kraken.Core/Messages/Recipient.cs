using FluentValidation;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Messages;

public record Recipient
{
  public RecipientType Type { get; }

  public string? Address { get; }
  public string? DisplayName { get; }

  public string? PhoneNumber { get; }

  public UserId? UserId { get; }
  [JsonIgnore]
  public User? User { get; }

  [JsonConstructor]
  public Recipient(RecipientType type = RecipientType.To, string? address = null, string? displayName = null, string? phoneNumber = null, UserId? userId = null)
  {
    Type = type;
    Address = address?.CleanTrim();
    DisplayName = displayName?.CleanTrim();
    PhoneNumber = phoneNumber?.CleanTrim();
    UserId = userId;
    new Validator().ValidateAndThrow(this);
  }

  public Recipient(User user, RecipientType type = RecipientType.To)
    : this(type, user.Email?.Address, user.FullName, user.Phone?.FormatToE164(), user.Id)
  {
    User = user;
  }

  private class Validator : AbstractValidator<Recipient>
  {
    public Validator()
    {
      RuleFor(x => x.Type).IsInEnum();

      RuleFor(x => x).Must(x => x.Address != null || x.PhoneNumber != null)
        .WithErrorCode("RecipientValidator")
        .WithMessage(x => $"At least one of the following must be specified: {nameof(x.Address)}, {nameof(x.PhoneNumber)}.");

      When(x => x.Address != null, () => RuleFor(x => x.Address!).EmailAddressInput());
      When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName).NotEmpty());

      When(x => x.PhoneNumber != null, () => RuleFor(x => x.PhoneNumber!).PhoneNumber());

      When(x => x.User != null, () => RuleFor(x => x.UserId).NotNull());
    }
  }
}
