using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

internal class MailgunSettingsValidator : AbstractValidator<IMailgunSettings>
{
  public MailgunSettingsValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty();
    RuleFor(x => x.DomainName).NotEmpty();
  }
}
