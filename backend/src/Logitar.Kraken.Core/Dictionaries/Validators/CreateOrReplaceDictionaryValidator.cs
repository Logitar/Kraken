using FluentValidation;
using Logitar.Kraken.Contracts.Dictionaries;

namespace Logitar.Kraken.Core.Dictionaries.Validators;

internal class CreateOrReplaceDictionaryValidator : AbstractValidator<CreateOrReplaceDictionaryPayload>
{
  public CreateOrReplaceDictionaryValidator()
  {
    RuleFor(x => x.Language).NotEmpty();
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryValidator());
  }
}
