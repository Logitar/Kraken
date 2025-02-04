using Moq;

namespace Logitar.Kraken.Core.Templates;

[Trait(Traits.Category, Categories.Unit)]
public class TemplateManagerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ITemplateQuerier> _templateQuerier = new();
  private readonly Mock<ITemplateRepository> _templateRepository = new();

  private readonly TemplateManager _manager;

  public TemplateManagerTests()
  {
    _manager = new(_templateQuerier.Object, _templateRepository.Object);
  }

  [Fact(DisplayName = "It should save a template.")]
  public async Task Given_NoChange_When_SaveAsync_Then_Saved()
  {
    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    TemplateContent content = TemplateContent.PlainText("{Token}");
    Template template = new(uniqueKey, subject, content);
    template.ClearChanges();

    await _manager.SaveAsync(template, _cancellationToken);

    _templateQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CancellationToken>()), Times.Never);
    _templateRepository.Verify(x => x.SaveAsync(template, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should save the template when there is no unique name conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved(bool found)
  {
    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    TemplateContent content = TemplateContent.PlainText("{Token}");
    Template template = new(uniqueKey, subject, content);
    if (found)
    {
      _templateQuerier.Setup(x => x.FindIdAsync(template.UniqueKey, _cancellationToken)).ReturnsAsync(template.Id);
    }

    await _manager.SaveAsync(template, _cancellationToken);

    _templateQuerier.Verify(x => x.FindIdAsync(template.UniqueKey, _cancellationToken), Times.Once);
    _templateRepository.Verify(x => x.SaveAsync(template, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw UniqueKeyAlreadyUsedException when the unique name is already used.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueKeyAlreadyUsedException()
  {
    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    TemplateContent content = TemplateContent.PlainText("{Token}");
    Template template = new(uniqueKey, subject, content);

    TemplateId conflictId = TemplateId.NewId(realmId: null);
    _templateQuerier.Setup(x => x.FindIdAsync(template.UniqueKey, _cancellationToken)).ReturnsAsync(conflictId);

    var exception = await Assert.ThrowsAsync<UniqueKeyAlreadyUsedException>(async () => await _manager.SaveAsync(template, _cancellationToken));
    Assert.Equal(template.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(conflictId.EntityId, exception.ConflictId);
    Assert.Equal(template.EntityId, exception.TemplateId);
    Assert.Equal(template.UniqueKey.Value, exception.UniqueKey);
    Assert.Equal("UniqueKey", exception.PropertyName);
  }
}
