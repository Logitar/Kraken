using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateContentTypeCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentTypeManager> _contentTypeManager = new();
  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();

  private readonly UpdateContentTypeCommandHandler _handler;

  public UpdateContentTypeCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentTypeManager.Object, _contentTypeQuerier.Object, _contentTypeRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Fact(DisplayName = "It should return null when updating a content type that does not exist.")]
  public async Task Given_NotExists_When_Handle_Then_NullReturned()
  {
    UpdateContentTypePayload payload = new();
    UpdateContentTypeCommand command = new(Guid.NewGuid(), payload);
    ContentTypeModel? contentType = await _handler.Handle(command, _cancellationToken);
    Assert.Null(contentType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    UpdateContentTypePayload payload = new()
    {
      UniqueName = "123_BlogArticle!",
      DisplayName = new ChangeModel<string>(RandomStringGenerator.GetString(999))
    };
    UpdateContentTypeCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName.Value");
  }

  [Fact(DisplayName = "It should update an existing content type.")]
  public async Task Given_Exists_When_Handle_Then_ContentTypeUpdated()
  {
    Identifier uniqueName = new("BlogArticle");
    ContentType contentType = new(uniqueName, isInvariant: true);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(contentType, _cancellationToken)).ReturnsAsync(model);

    UpdateContentTypePayload payload = new()
    {
      IsInvariant = false,
      DisplayName = new ChangeModel<string>(" Blog Article "),
      Description = new ChangeModel<string>("  This is the content type for blog articles.  ")
    };
    UpdateContentTypeCommand command = new(contentType.EntityId, payload);
    ContentTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _contentTypeManager.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, contentType.UpdatedBy);
    Assert.Equal(payload.IsInvariant, contentType.IsInvariant);
    Assert.Equal(uniqueName, contentType.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), contentType.DisplayName?.Value);
    Assert.Equal(payload.Description.Value?.Trim(), contentType.Description?.Value);
  }
}
