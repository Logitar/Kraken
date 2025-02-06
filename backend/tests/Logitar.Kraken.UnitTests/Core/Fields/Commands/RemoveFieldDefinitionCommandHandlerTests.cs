using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RemoveFieldDefinitionCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();

  private readonly RemoveFieldDefinitionCommandHandler _handler;

  private readonly ContentType _contentType;

  public RemoveFieldDefinitionCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentTypeQuerier.Object, _contentTypeRepository.Object);

    _contentType = new(new Identifier("BlogArticle"), isInvariant: false, actorId: null, ContentTypeId.NewId(_realmId));
    _contentTypeRepository.Setup(x => x.LoadAsync(_contentType.Id, _cancellationToken)).ReturnsAsync(_contentType);

    _applicationContext.SetupGet(x => x.ActorId).Returns(_actorId);
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);
  }

  [Fact(DisplayName = "It should remove an existing field definition.")]
  public async Task Given_Found_When_Handle_Then_Removed()
  {
    FieldDefinition fieldDefinition = new(
      Guid.NewGuid(),
      FieldTypeId.NewId(_realmId),
      IsInvariant: false,
      IsRequired: false,
      IsIndexed: false,
      IsUnique: false,
      new Identifier("ArticleTitle"),
      DisplayName: null,
      Description: null,
      Placeholder: null);
    _contentType.SetField(fieldDefinition);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(_contentType, _cancellationToken)).ReturnsAsync(model);

    RemoveFieldDefinitionCommand command = new(_contentType.EntityId, fieldDefinition.Id);
    ContentTypeModel? contentType = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Same(model, contentType);

    Assert.Null(_contentType.TryGetField(fieldDefinition.Id));
    Assert.Contains(_contentType.Changes, change => change is ContentTypeFieldDefinitionRemoved removed && removed.FieldId == fieldDefinition.Id && removed.ActorId == _actorId);

    _contentTypeRepository.Verify(x => x.SaveAsync(_contentType, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the content type was not found.")]
  public async Task Given_ContentTypeNotFound_When_Handle_Then_NullReturned()
  {
    RemoveFieldDefinitionCommand command = new(Guid.NewGuid(), Guid.NewGuid());
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should return null when the field definition was not found.")]
  public async Task Given_FieldDefinitionNotFound_When_Handle_Then_NullReturned()
  {
    RemoveFieldDefinitionCommand command = new(_contentType.EntityId, Guid.NewGuid());
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }
}
