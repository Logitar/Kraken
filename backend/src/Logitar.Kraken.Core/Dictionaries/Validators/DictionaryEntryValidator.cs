using FluentValidation;
using Logitar.Kraken.Contracts.Dictionaries;

namespace Logitar.Kraken.Core.Dictionaries.Validators;

internal class DictionaryEntryValidator : AbstractValidator<DictionaryEntryModel>
{
  public DictionaryEntryValidator()
  {
    RuleFor(x => x.Key).Identifier();
  }
}
