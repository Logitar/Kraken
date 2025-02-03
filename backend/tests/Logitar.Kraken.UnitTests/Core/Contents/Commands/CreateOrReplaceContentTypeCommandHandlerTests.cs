using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Realms;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceContentTypeCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentTypeManager> _contentTypeManager = new();
  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();

  private readonly CreateOrReplaceContentTypeCommandHandler _handler;

  public CreateOrReplaceContentTypeCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentTypeManager.Object, _contentTypeQuerier.Object, _contentTypeRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Theory(DisplayName = "It should create a new invariant content type.")]
  [InlineData(null)]
  [InlineData("09c77545-0303-4bc6-84ce-89abccd84592")]
  public async Task Given_InvariantNotExists_When_Handle_Then_ContentTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    RealmId realmId = RealmId.NewId();
    _applicationContext.Setup(x => x.RealmId).Returns(realmId);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<ContentType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentTypePayload payload = new()
    {
      IsInvariant = true,
      UniqueName = "BlogCategory",
      DisplayName = " Blog Category ",
      Description = "  This is the content type for blog categories.  "
    };
    CreateOrReplaceContentTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.ContentType);
    Assert.Same(model, result.ContentType);

    _contentTypeManager.Verify(x => x.SaveAsync(
      It.Is<ContentType>(y => y.RealmId == realmId
        && (!id.HasValue || id.Value == y.EntityId)
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.IsInvariant && y.UniqueName.Value == payload.UniqueName
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value.Trim() == payload.Description.Trim()),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new variant content type.")]
  [InlineData(null)]
  [InlineData("8080365d-3056-4dcd-9145-064ddef02185")]
  public async Task Given_VariantNotExists_When_Handle_Then_ContentTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<ContentType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentTypePayload payload = new()
    {
      IsInvariant = false,
      UniqueName = "BlogArticle",
      DisplayName = " Blog Article ",
      Description = "  This is the content type for blog articles.  "
    };
    CreateOrReplaceContentTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.ContentType);
    Assert.Same(model, result.ContentType);

    _contentTypeManager.Verify(x => x.SaveAsync(
      It.Is<ContentType>(y => (!id.HasValue || id.Value == y.EntityId)
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && !y.IsInvariant && y.UniqueName.Value == payload.UniqueName
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value.Trim() == payload.Description.Trim()),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should replace/update an existing invariant content type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_InariantExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    ContentType contentType = new(new Identifier("BlogAuthor"), isInvariant: true);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    long? version = null;
    DisplayName? displayName = null;
    if (update)
    {
      version = contentType.Version;

      ContentType reference = new(contentType.UniqueName, contentType.IsInvariant, contentType.CreatedBy, contentType.Id);
      _contentTypeRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

      displayName = new("Blog Author");
      contentType.DisplayName = displayName;
      contentType.Update();
    }

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(contentType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentTypePayload payload = new()
    {
      IsInvariant = false,
      UniqueName = "BlogAuthor",
      DisplayName = null,
      Description = "  This is the content type for blog authors.  "
    };
    CreateOrReplaceContentTypeCommand command = new(contentType.EntityId, payload, version);
    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.ContentType);
    Assert.Same(model, result.ContentType);

    _contentTypeManager.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, contentType.UpdatedBy);
    Assert.False(contentType.IsInvariant);
    Assert.Equal(payload.UniqueName, contentType.UniqueName.Value);
    Assert.Equal(displayName?.Value, contentType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), contentType.Description?.Value);
  }

  [Theory(DisplayName = "It should replace/update an existing variant content type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_VariantExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    long? version = null;
    DisplayName? displayName = null;
    if (update)
    {
      version = contentType.Version;

      ContentType reference = new(contentType.UniqueName, contentType.IsInvariant, contentType.CreatedBy, contentType.Id);
      _contentTypeRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

      displayName = new("Blog Author");
      contentType.DisplayName = displayName;
      contentType.Update();
    }

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(contentType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentTypePayload payload = new()
    {
      IsInvariant = true,
      UniqueName = "BlogArticle",
      DisplayName = null,
      Description = "  This is the content type for blog articles.  "
    };
    CreateOrReplaceContentTypeCommand command = new(contentType.EntityId, payload, version);
    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.ContentType);
    Assert.Same(model, result.ContentType);

    _contentTypeManager.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, contentType.UpdatedBy);
    Assert.True(contentType.IsInvariant);
    Assert.Equal(payload.UniqueName, contentType.UniqueName.Value);
    Assert.Equal(displayName?.Value, contentType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), contentType.Description?.Value);
  }

  [Fact(DisplayName = "It should return null when updating a content type that does not exist.")]
  public async Task Given_NotExistsWithVersion_When_Handle_Then_EmptyResult()
  {
    CreateOrReplaceContentTypePayload payload = new()
    {
      UniqueName = "BlogArticle"
    };
    CreateOrReplaceContentTypeCommand command = new(Guid.NewGuid(), payload, Version: -1);
    CreateOrReplaceContentTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.Null(result.ContentType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceContentTypePayload payload = new()
    {
      UniqueName = "123_BlogArticle!",
      DisplayName = RandomStringGenerator.GetString(999)
    };
    CreateOrReplaceContentTypeCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
  }
}
