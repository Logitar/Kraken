using FluentValidation;
using Logitar.Kraken.Contracts.Dictionaries;

namespace Logitar.Kraken.Core.Dictionaries.Validators;

internal class UpdateDictionaryValidator : AbstractValidator<UpdateDictionaryPayload>
{
  public UpdateDictionaryValidator()
  {
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryValidator());
  }
}
