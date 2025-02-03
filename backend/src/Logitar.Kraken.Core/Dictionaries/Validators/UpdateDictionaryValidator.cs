using FluentValidation;
using Logitar.Kraken.Contracts.Dictionaries;

namespace Logitar.Kraken.Core.Dictionaries.Validators;

internal class UpdateDictionaryValidator : AbstractValidator<UpdateDictionaryPayload>
{
  public UpdateDictionaryValidator()
  {
    // TODO(fpion): Language
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryValidator());
  }
}
