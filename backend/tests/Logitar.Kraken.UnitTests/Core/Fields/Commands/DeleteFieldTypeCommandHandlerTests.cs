using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteFieldTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly DeleteFieldTypeCommandHandler _handler;

  private readonly FieldType _fieldType;

  public DeleteFieldTypeCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentTypeRepository.Object, _fieldTypeQuerier.Object, _fieldTypeRepository.Object);

    UniqueName uniqueName = new(FieldType.UniqueNameSettings, "IsFeatured");
    BooleanProperties properties = new();
    _fieldType = new(uniqueName, properties, actorId: null, FieldTypeId.NewId(_realmId));
    _fieldTypeRepository.Setup(x => x.LoadAsync(_fieldType.Id, _cancellationToken)).ReturnsAsync(_fieldType);
  }

  [Fact(DisplayName = "It should delete an existing field type.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_fieldType, _cancellationToken)).ReturnsAsync(model);

    FieldTypeId otherFieldTypeId = FieldTypeId.NewId(_realmId);

    ContentType contentType1 = new(new Identifier("BlogAuthor"), isInvariant: true, actorId: null, ContentTypeId.NewId(_realmId));
    contentType1.SetField(new FieldDefinition(Guid.NewGuid(), _fieldType.Id, IsInvariant: true, IsRequired: true, IsIndexed: true, IsUnique: false, new Identifier("IsFeatured"), DisplayName: null, Description: null, Placeholder: null));
    contentType1.SetField(new FieldDefinition(Guid.NewGuid(), otherFieldTypeId, IsInvariant: true, IsRequired: false, IsIndexed: false, IsUnique: false, new Identifier("WebsiteUrl"), DisplayName: null, Description: null, Placeholder: null));
    ContentType contentType2 = new(new Identifier("BlogArticle"), isInvariant: false, actorId: null, ContentTypeId.NewId(_realmId));
    contentType2.SetField(new FieldDefinition(Guid.NewGuid(), _fieldType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false, new Identifier("IsFeatured1"), DisplayName: null, Description: null, Placeholder: null));
    contentType2.SetField(new FieldDefinition(Guid.NewGuid(), _fieldType.Id, IsInvariant: false, IsRequired: false, IsIndexed: true, IsUnique: false, new Identifier("IsFeatured2"), DisplayName: null, Description: null, Placeholder: null));
    ContentType[] contentTypes = [contentType1, contentType2];
    _contentTypeRepository.Setup(x => x.LoadAsync(_fieldType.Id, _cancellationToken)).ReturnsAsync(contentTypes);

    DeleteFieldTypeCommand command = new(_fieldType.EntityId);
    FieldTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Equal(otherFieldTypeId, Assert.Single(contentType1.FieldDefinitions).FieldTypeId);
    Assert.Equal(1, contentType1.Changes.Count(change => change is ContentTypeFieldDefinitionRemoved removed && removed.ActorId == actorId));
    Assert.Empty(contentType2.FieldDefinitions);
    Assert.Equal(2, contentType2.Changes.Count(change => change is ContentTypeFieldDefinitionRemoved removed && removed.ActorId == actorId));
    Assert.True(_fieldType.IsDeleted);

    _contentTypeRepository.Verify(x => x.SaveAsync(contentTypes, _cancellationToken), Times.Once());
    _fieldTypeRepository.Verify(x => x.SaveAsync(_fieldType, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the field type could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteFieldTypeCommand command = new(_fieldType.EntityId);
    FieldTypeModel? fieldType = await _handler.Handle(command, _cancellationToken);
    Assert.Null(fieldType);
  }
}
