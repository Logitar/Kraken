using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.Fields;

[Trait(Traits.Category, Categories.Unit)]
public class FieldTypeManagerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly FieldTypeManager _manager;

  public FieldTypeManagerTests()
  {
    _manager = new(_contentTypeQuerier.Object, _fieldTypeQuerier.Object, _fieldTypeRepository.Object);
  }

  [Fact(DisplayName = "It should save a field type.")]
  public async Task Given_NoChange_When_SaveAsync_Then_Saved()
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties());
    fieldType.ClearChanges();

    await _manager.SaveAsync(fieldType, _cancellationToken);

    _fieldTypeQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _contentTypeQuerier.Verify(x => x.ReadAsync(It.IsAny<ContentTypeId>(), It.IsAny<CancellationToken>()), Times.Never);
    _fieldTypeRepository.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should save the field type when there is no unique name conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved(bool found)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties());
    if (found)
    {
      _fieldTypeQuerier.Setup(x => x.FindIdAsync(fieldType.UniqueName, _cancellationToken)).ReturnsAsync(fieldType.Id);
    }

    await _manager.SaveAsync(fieldType, _cancellationToken);

    _fieldTypeQuerier.Verify(x => x.FindIdAsync(fieldType.UniqueName, _cancellationToken), Times.Once);
    _fieldTypeRepository.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should save the field type when the related content type exists.")]
  public async Task Given_ContentTypeExists_When_SaveAsync_Then_Saved()
  {
    ContentType author = new(new Identifier("Author"));
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "Authors"), new RelatedContentProperties(author.Id, isMultiple: true));

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(author.Id, _cancellationToken)).ReturnsAsync(model);

    await _manager.SaveAsync(fieldType, _cancellationToken);

    _contentTypeQuerier.Verify(x => x.ReadAsync(author.Id, _cancellationToken), Times.Once);
    _fieldTypeRepository.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ContentTypeNotFoundException when the related content type does not exist.")]
  public async Task Given_ContentTypeNotExists_When_SaveAsync_Then_ContentTypeNotFoundException()
  {
    RelatedContentProperties settings = new(ContentTypeId.NewId(realmId: null));
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), settings);

    var exception = await Assert.ThrowsAsync<ContentTypeNotFoundException>(async () => await _manager.SaveAsync(fieldType, _cancellationToken));
    Assert.Equal(settings.ContentTypeId.EntityId, exception.ContentTypeId);
    Assert.Equal("ContentTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    RealmId realmId = RealmId.NewId();
    FieldTypeId fieldTypeId = FieldTypeId.NewId(realmId);
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(), actorId: null, fieldTypeId);

    FieldTypeId conflictId = FieldTypeId.NewId(realmId: null);
    _fieldTypeQuerier.Setup(x => x.FindIdAsync(fieldType.UniqueName, _cancellationToken)).ReturnsAsync(conflictId);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _manager.SaveAsync(fieldType, _cancellationToken));
    Assert.Equal(realmId.ToGuid(), exception.RealmId);
    Assert.Equal(conflictId.EntityId, exception.ConflictId);
    Assert.Equal(fieldType.Id.EntityId, exception.EntityId);
    Assert.Equal(fieldType.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
