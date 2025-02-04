using FluentValidation;
using Logitar.Kraken.Contracts.Dictionaries;

namespace Logitar.Kraken.Core.Dictionaries.Validators;

internal class UpdateDictionaryValidator : AbstractValidator<UpdateDictionaryPayload>
{
  public UpdateDictionaryValidator()
  {
    // ISSUE #44: https://github.com/Logitar/Kraken/issues/44
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryValidator());
  }
}
