using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceFieldDefinitionCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly CreateOrReplaceFieldDefinitionCommandHandler _handler;

  public CreateOrReplaceFieldDefinitionCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentTypeQuerier.Object, _contentTypeRepository.Object, _fieldTypeRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Theory(DisplayName = "It should create a new field definition.")]
  [InlineData(null)]
  [InlineData("f9ab262f-88b6-44c0-a2ce-4968f03b47fc")]
  public async Task Given_FieldNotExists_When_Handle_Then_FieldDefinitionCreated(string? idValue)
  {
    Guid? fieldId = idValue == null ? null : Guid.Parse(idValue);

    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(minimumLength: 1, maximumLength: 100));
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(contentType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      FieldTypeId = fieldType.EntityId,
      IsRequired = true,
      IsIndexed = true,
      UniqueName = "ArticleTitle",
      DisplayName = " Article Title ",
      Description = "  This is the field for article titles.  ",
      Placeholder = "   Enter the title of the article.   "
    };
    CreateOrReplaceFieldDefinitionCommand command = new(contentType.EntityId, fieldId, payload);
    ContentTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _contentTypeRepository.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);
    Assert.Equal(_actorId, contentType.UpdatedBy);

    FieldDefinition field = Assert.Single(contentType.FieldDefinitions);
    if (fieldId.HasValue)
    {
      Assert.Equal(fieldId.Value, field.Id);
    }
    Assert.Equal(fieldType.Id, field.FieldTypeId);
    Assert.Equal(payload.IsInvariant, field.IsInvariant);
    Assert.Equal(payload.IsRequired, field.IsRequired);
    Assert.Equal(payload.IsIndexed, field.IsIndexed);
    Assert.Equal(payload.IsUnique, field.IsUnique);
    Assert.Equal(payload.UniqueName, field.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), field.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), field.Description?.Value);
    Assert.Equal(payload.Placeholder.Trim(), field.Placeholder?.Value);
  }

  [Fact(DisplayName = "It should replace an existing field definition.")]
  public async Task Given_FieldExists_When_Handle_Then_FieldDefinitionReplaced()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(minimumLength: 1, maximumLength: 100));
    Guid fieldId = Guid.NewGuid();
    FieldDefinition fieldDefinition = new(fieldId, fieldType.Id, IsInvariant: true, IsRequired: false, IsIndexed: false, IsUnique: true,
      new Identifier("Article_Title"), DisplayName: null, Description: null, Placeholder: null);
    contentType.SetField(fieldDefinition);

    ContentTypeModel model = new();
    _contentTypeQuerier.Setup(x => x.ReadAsync(contentType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      IsInvariant = false,
      IsRequired = true,
      IsIndexed = true,
      IsUnique = false,
      UniqueName = "ArticleTitle",
      DisplayName = " Article Title ",
      Description = "  This is the field for article titles.  ",
      Placeholder = "   Enter the title of the article.   "
    };
    CreateOrReplaceFieldDefinitionCommand command = new(contentType.EntityId, fieldId, payload);
    ContentTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _contentTypeRepository.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);
    Assert.Equal(_actorId, contentType.UpdatedBy);

    FieldDefinition field = Assert.Single(contentType.FieldDefinitions);
    Assert.Equal(fieldId, field.Id);
    Assert.Equal(fieldType.Id, field.FieldTypeId);
    Assert.Equal(payload.IsInvariant, field.IsInvariant);
    Assert.Equal(payload.IsRequired, field.IsRequired);
    Assert.Equal(payload.IsIndexed, field.IsIndexed);
    Assert.Equal(payload.IsUnique, field.IsUnique);
    Assert.Equal(payload.UniqueName, field.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), field.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), field.Description?.Value);
    Assert.Equal(payload.Placeholder.Trim(), field.Placeholder?.Value);
  }

  [Fact(DisplayName = "It should return null when the content type could not be found.")]
  public async Task Given_ContentTypeNotExists_When_Handle_Then_NullReturned()
  {
    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      UniqueName = "ArticleTitle"
    };
    CreateOrReplaceFieldDefinitionCommand command = new(Guid.NewGuid(), FieldId: null, payload);
    ContentTypeModel? contentType = await _handler.Handle(command, _cancellationToken);
    Assert.Null(contentType);
  }

  [Fact(DisplayName = "It should throw FieldTypeNotFoundException when the field type could not be found.")]
  public async Task Given_FieldDefinitionNotExists_When_Handle_Then_FieldTypeNotFoundException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      FieldTypeId = Guid.NewGuid(),
      UniqueName = "ArticleTitle"
    };
    CreateOrReplaceFieldDefinitionCommand command = new(contentType.EntityId, FieldId: null, payload);
    var exception = await Assert.ThrowsAsync<FieldTypeNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.FieldTypeId, exception.FieldTypeId);
    Assert.Equal("FieldTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when creating a field definition, but the field type ID is null.")]
  public async Task Given_NewFieldDefinitionFieldTypeIdNull_When_Handle_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      UniqueName = "ArticleTitle"
    };
    CreateOrReplaceFieldDefinitionCommand command = new(contentType.EntityId, FieldId: null, payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("RequiredValidator", error.ErrorCode);
    Assert.Equal("FieldTypeId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      UniqueName = "123_Test",
      DisplayName = RandomStringGenerator.GetString(999),
      Placeholder = RandomStringGenerator.GetString(999)
    };
    CreateOrReplaceFieldDefinitionCommand command = new(Guid.NewGuid(), FieldId: null, payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Placeholder");
  }
}
