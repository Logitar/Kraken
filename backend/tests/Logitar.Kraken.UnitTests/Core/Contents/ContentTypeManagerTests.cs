using Moq;

namespace Logitar.Kraken.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeManagerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();

  private readonly ContentTypeManager _manager;

  public ContentTypeManagerTests()
  {
    _manager = new(_contentTypeQuerier.Object, _contentTypeRepository.Object);
  }

  [Fact(DisplayName = "It should save a content type.")]
  public async Task Given_NoChange_When_SaveAsync_Then_Saved()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"), isInvariant: true);
    contentType.ClearChanges();

    await _manager.SaveAsync(contentType, _cancellationToken);

    _contentTypeQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CancellationToken>()), Times.Never);
    _contentTypeRepository.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should save the content type when there is no unique name conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved(bool found)
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    if (found)
    {
      _contentTypeQuerier.Setup(x => x.FindIdAsync(contentType.UniqueName, _cancellationToken)).ReturnsAsync(contentType.Id);
    }

    await _manager.SaveAsync(contentType, _cancellationToken);

    _contentTypeQuerier.Verify(x => x.FindIdAsync(contentType.UniqueName, _cancellationToken), Times.Once);
    _contentTypeRepository.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);

    ContentTypeId conflictId = ContentTypeId.NewId(realmId: null);
    _contentTypeQuerier.Setup(x => x.FindIdAsync(contentType.UniqueName, _cancellationToken)).ReturnsAsync(conflictId);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _manager.SaveAsync(contentType, _cancellationToken));
    Assert.Equal(contentType.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(conflictId.EntityId, exception.ConflictId);
    Assert.Equal(contentType.EntityId, exception.EntityId);
    Assert.Equal(contentType.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
