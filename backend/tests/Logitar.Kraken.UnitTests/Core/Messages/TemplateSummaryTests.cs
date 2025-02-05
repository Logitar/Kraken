using Logitar.Kraken.Core.Templates;

namespace Logitar.Kraken.Core.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class TemplateSummaryTests
{
  [Fact(DisplayName = "It should construct the correct instance from a template.")]
  public void Given_Template_When_ctor_Then_Constructed()
  {
    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    TemplateContent content = TemplateContent.PlainText("{Token}");
    Template template = new(uniqueKey, subject, content);

    TemplateSummary summary = new(template);

    Assert.Equal(template.Id, summary.Id);
    Assert.Equal(template.UniqueKey, summary.UniqueKey);
    Assert.Equal(template.DisplayName, summary.DisplayName);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    TemplateId id = TemplateId.NewId(realmId: null);
    Identifier uniqueKey = new("PasswordRecovery");
    DisplayName displayName = new("Password Recovery");

    TemplateSummary template = new(id, uniqueKey, displayName);

    Assert.Equal(id, template.Id);
    Assert.Equal(uniqueKey, template.UniqueKey);
    Assert.Equal(displayName, template.DisplayName);
  }
}
