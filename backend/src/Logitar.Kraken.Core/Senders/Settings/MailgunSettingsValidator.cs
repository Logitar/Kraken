using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

public class MailgunSettingsValidator : AbstractValidator<IMailgunSettings>
{
  public MailgunSettingsValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty();
    RuleFor(x => x.DomainName).NotEmpty();
  }
}
