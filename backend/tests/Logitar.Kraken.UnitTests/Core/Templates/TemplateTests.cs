using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Templates.Events;

namespace Logitar.Kraken.Core.Templates;

[Trait(Traits.Category, Categories.Unit)]
public class TemplateTests
{
  private readonly Template _template;

  public TemplateTests()
  {
    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    TemplateContent content = TemplateContent.PlainText("{Token}");
    _template = new(uniqueKey, subject, content);
  }

  [Fact(DisplayName = "Content: it should handle the updates correctly.")]
  public void Given_DisplayNameUpdates_When_setContent_Then_UpdatesHandledCorrectly()
  {
    _template.ClearChanges();

    _template.Content = _template.Content;
    _template.Update();
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);

    _template.Content = _template.Content.Create("Your password reset token is: {Token}");
    _template.Update();
    Assert.True(_template.HasChanges);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.Content == _template.Content);
  }

  [Fact(DisplayName = "Delete: it should delete the template.")]
  public void Given_Template_When_Delete_Then_Deleted()
  {
    _template.Delete();
    Assert.True(_template.IsDeleted);

    _template.ClearChanges();
    _template.Delete();
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);
  }

  [Fact(DisplayName = "Description: it should handle the updates correctly.")]
  public void Given_DescriptionUpdates_When_setDescription_Then_UpdatesHandledCorrectly()
  {
    _template.ClearChanges();

    _template.Description = null;
    _template.Update();
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);

    _template.Description = new Description("This is a new template.");
    _template.Update();
    Assert.True(_template.HasChanges);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.Description?.Value == _template.Description);
  }

  [Fact(DisplayName = "DisplayName: it should handle the updates correctly.")]
  public void Given_DisplayNameUpdates_When_setDisplayName_Then_UpdatesHandledCorrectly()
  {
    _template.ClearChanges();

    _template.DisplayName = _template.DisplayName;
    _template.Update();
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);

    _template.DisplayName = new DisplayName("New API Key");
    _template.Update();
    Assert.True(_template.HasChanges);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.DisplayName?.Value == _template.DisplayName);
  }

  [Fact(DisplayName = "It should construct the correct template.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    TemplateContent content = TemplateContent.PlainText("{Token}");
    ActorId actorId = ActorId.NewId();
    RealmId realmId = RealmId.NewId();
    Guid entityId = Guid.NewGuid();
    TemplateId templateId = new(realmId, entityId);

    Template template = new(uniqueKey, subject, content, actorId, templateId);

    Assert.Equal(actorId, template.CreatedBy);
    Assert.Equal(actorId, template.UpdatedBy);
    Assert.Equal(templateId, template.Id);
    Assert.Equal(realmId, template.RealmId);
    Assert.Equal(entityId, template.EntityId);
    Assert.Equal(uniqueKey, template.UniqueKey);
    Assert.Equal(subject, template.Subject);
    Assert.Equal(content, template.Content);
  }

  [Fact(DisplayName = "SetUniqueKey: it should handle the updated correctly.")]
  public void Given_UniqueKeyUpdates_When_setSetUniqueKey_Then_UpdatesHandledCorrectly()
  {
    Identifier uniqueKey = new("AccountActivation");
    ActorId actorId = ActorId.NewId();
    _template.SetUniqueKey(uniqueKey, actorId);
    Assert.Contains(_template.Changes, change => change is TemplateUniqueKeyChanged changed && changed.UniqueKey == uniqueKey && changed.ActorId == actorId);
    Assert.Equal(actorId, _template.UpdatedBy);

    _template.ClearChanges();
    _template.SetUniqueKey(uniqueKey);
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);
  }

  [Fact(DisplayName = "Subject: it should handle the updates correctly.")]
  public void Given_DisplayNameUpdates_When_setSubject_Then_UpdatesHandledCorrectly()
  {
    _template.ClearChanges();

    _template.Subject = _template.Subject;
    _template.Update();
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);

    _template.Subject = new Subject("Subject");
    _template.Update();
    Assert.True(_template.HasChanges);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.Subject == _template.Subject);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(null)]
  [InlineData("Password Recovery")]
  public void Given_Template_When_ToString_Then_CorrectString(string? displayName)
  {
    if (displayName == null)
    {
      Assert.StartsWith(_template.UniqueKey.Value, _template.ToString());
    }
    else
    {
      _template.DisplayName = new(displayName);
      Assert.StartsWith(_template.DisplayName.Value, _template.ToString());
    }
  }

  [Theory(DisplayName = "Update: it should update the template.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_TemplateUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _template.ClearChanges();
    _template.Update();
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);

    _template.DisplayName = new DisplayName("Password Recovery");
    _template.Update(actorId);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }
}
