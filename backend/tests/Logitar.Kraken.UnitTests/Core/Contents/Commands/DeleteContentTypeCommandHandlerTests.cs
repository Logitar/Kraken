using Bogus;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteContentTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();

  private readonly DeleteContentTypeCommandHandler _handler;

  private readonly ContentType _contentType;

  public DeleteContentTypeCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentRepository.Object, _contentTypeQuerier.Object, _contentTypeRepository.Object);

    _contentType = new(new Identifier("BlogArticle"), isInvariant: false, actorId: null, ContentTypeId.NewId(_realmId));
    _contentTypeRepository.Setup(x => x.LoadAsync(_contentType.Id, _cancellationToken)).ReturnsAsync(_contentType);
  }

  [Fact(DisplayName = "It should delete an existing content type.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(_contentType, _cancellationToken)).ReturnsAsync(model);

    Content[] contents = [new(_contentType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName)), contentId: ContentId.NewId(_realmId))];
    _contentRepository.Setup(x => x.LoadAsync(_contentType.Id, _cancellationToken)).ReturnsAsync(contents);

    DeleteContentTypeCommand command = new(_contentType.EntityId);
    ContentTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(contents.All(session => session.IsDeleted));
    Assert.True(_contentType.IsDeleted);

    _contentRepository.Verify(x => x.SaveAsync(contents, _cancellationToken), Times.Once());
    _contentTypeRepository.Verify(x => x.SaveAsync(_contentType, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the content type could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteContentTypeCommand command = new(_contentType.EntityId);
    ContentTypeModel? contentType = await _handler.Handle(command, _cancellationToken);
    Assert.Null(contentType);
  }
}
