using FluentValidation;
using Logitar.Kraken.Web.Models.Account;

namespace Logitar.Kraken.Web.Validators.Account;

internal class GetTokenValidator : AbstractValidator<GetTokenPayload>
{
  public GetTokenValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.RefreshToken), () =>
    {
      RuleFor(x => x.Username).Empty();
      RuleFor(x => x.Password).Empty();
    });

    When(x => !string.IsNullOrWhiteSpace(x.Username), () =>
    {
      RuleFor(x => x.RefreshToken).Empty();
      RuleFor(x => x.Password).NotEmpty();
    });
    When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
    {
      RuleFor(x => x.RefreshToken).Empty();
      RuleFor(x => x.Username).NotEmpty();
    });
  }
}
