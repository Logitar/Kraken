using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateFieldDefinitionCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();

  private readonly UpdateFieldDefinitionCommandHandler _handler;

  public UpdateFieldDefinitionCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentTypeQuerier.Object, _contentTypeRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Fact(DisplayName = "It should return null when the content type could not be found.")]
  public async Task Given_ContentTypeNotFound_When_Handle_Then_NullReturned()
  {
    UpdateFieldDefinitionPayload payload = new();
    UpdateFieldDefinitionCommand command = new(Guid.NewGuid(), Guid.NewGuid(), payload);
    ContentTypeModel? contentType = await _handler.Handle(command, _cancellationToken);
    Assert.Null(contentType);
  }

  [Fact(DisplayName = "It should return null when the field definition could not be found.")]
  public async Task Given_FieldDefinitionNotFound_When_Handle_Then_NullReturned()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    UpdateFieldDefinitionPayload payload = new();
    UpdateFieldDefinitionCommand command = new(contentType.EntityId, Guid.NewGuid(), payload);
    ContentTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    UpdateFieldDefinitionPayload payload = new()
    {
      UniqueName = "123_Test",
      DisplayName = new ChangeModel<string>(RandomStringGenerator.GetString(999)),
      Placeholder = new ChangeModel<string>(RandomStringGenerator.GetString(999))
    };
    UpdateFieldDefinitionCommand command = new(Guid.NewGuid(), Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Placeholder.Value");
  }

  [Fact(DisplayName = "It should update an existing field definition.")]
  public async Task Given_FieldDefinitionExists_When_Handle_Then_Updated()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(minimumLength: 1, maximumLength: 100));
    FieldDefinition field = new(Guid.NewGuid(), fieldType.Id, IsInvariant: false, IsRequired: false, IsIndexed: false, IsUnique: false,
      new Identifier("ArticleTitle"), DisplayName: null, Description: null, Placeholder: null);
    contentType.SetField(field);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(contentType, _cancellationToken)).ReturnsAsync(model);

    UpdateFieldDefinitionPayload payload = new()
    {
      IsRequired = true,
      IsIndexed = true,
      DisplayName = new ChangeModel<string>(" Article Title "),
      Description = new ChangeModel<string>("  This is the field for article titles.  "),
      Placeholder = new ChangeModel<string>("   Enter the title of the article.   ")
    };
    UpdateFieldDefinitionCommand command = new(contentType.EntityId, field.Id, payload);
    ContentTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _contentTypeRepository.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);
    Assert.Equal(_actorId, contentType.UpdatedBy);

    FieldDefinition changedField = Assert.Single(contentType.FieldDefinitions);
    Assert.Equal(field.Id, changedField.Id);
    Assert.Equal(field.FieldTypeId, changedField.FieldTypeId);
    Assert.Equal(field.IsInvariant, changedField.IsInvariant);
    Assert.Equal(payload.IsRequired, changedField.IsRequired);
    Assert.Equal(payload.IsIndexed, changedField.IsIndexed);
    Assert.Equal(field.IsUnique, changedField.IsUnique);
    Assert.Equal(field.UniqueName, changedField.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), changedField.DisplayName?.Value);
    Assert.Equal(payload.Description.Value?.Trim(), changedField.Description?.Value);
    Assert.Equal(payload.Placeholder.Value?.Trim(), changedField.Placeholder?.Value);
  }
}
