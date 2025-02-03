using FluentValidation;
using Logitar.Kraken.Contracts.Dictionaries;

namespace Logitar.Kraken.Core.Dictionaries.Validators;

internal class CreateOrReplaceDictionaryValidator : AbstractValidator<CreateOrReplaceDictionaryPayload>
{
  public CreateOrReplaceDictionaryValidator()
  {
    // TODO(fpion): Language
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryValidator());
  }
}
