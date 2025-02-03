using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class UpdateFieldTypeValidator : AbstractValidator<UpdateFieldTypePayload>
{
  public UpdateFieldTypeValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(FieldType.UniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Boolean != null, () => RuleFor(x => x.Boolean!).SetValidator(new BooleanPropertiesValidator()));
    When(x => x.DateTime != null, () => RuleFor(x => x.DateTime!).SetValidator(new DateTimePropertiesValidator()));
    When(x => x.Number != null, () => RuleFor(x => x.Number!).SetValidator(new NumberPropertiesValidator()));
    When(x => x.RelatedContent != null, () => RuleFor(x => x.RelatedContent!).SetValidator(new RelatedContentPropertiesValidator()));
    When(x => x.RichText != null, () => RuleFor(x => x.RichText!).SetValidator(new RichTextPropertiesValidator()));
    When(x => x.Select != null, () => RuleFor(x => x.Select!).SetValidator(new SelectPropertiesValidator()));
    When(x => x.String != null, () => RuleFor(x => x.String!).SetValidator(new StringPropertiesValidator()));
    When(x => x.Tags != null, () => RuleFor(x => x.Tags!).SetValidator(new TagsPropertiesValidator()));
  }
}
