using Bogus;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Contents.Commands;

public class CreateOrReplaceContentCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentManager> _contentManager = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly CreateOrReplaceContentCommandHandler _handler;

  public CreateOrReplaceContentCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentManager.Object, _contentQuerier.Object, _contentRepository.Object, _contentTypeRepository.Object, _languageRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Theory(DisplayName = "It should create a new invariant content.")]
  [InlineData(null)]
  [InlineData("a5bcdba9-53e3-413c-9f32-c6e47b141670")]
  public async Task Given_InvariantNotExists_When_Handle_Then_ContentCreated(string? id)
  {
    Guid? contentId = id == null ? null : Guid.Parse(id);

    RealmId realmId = RealmId.NewId();
    _applicationContext.Setup(x => x.RealmId).Returns(realmId);

    ContentType contentType = new(new Identifier("BlogAuthor"), isInvariant: true, actorId: null, ContentTypeId.NewId(realmId));
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    ContentModel content = new();
    _contentQuerier.Setup(x => x.ReadAsync(It.IsAny<Content>(), _cancellationToken)).ReturnsAsync(content);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.Id.EntityId,
      UniqueName = _faker.Person.UserName
    };
    CreateOrReplaceContentCommand command = new(contentId, LanguageId: null, payload);
    CreateOrReplaceContentResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.Content);
    Assert.Same(content, result.Content);

    _contentManager.Verify(x => x.SaveAsync(
      It.Is<Content>(y => y.RealmId == realmId
        && (!contentId.HasValue || contentId.Value == y.Id.EntityId)
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.ContentTypeId == contentType.Id
        && y.Invariant.UniqueName.Value == payload.UniqueName
        && y.Invariant.DisplayName == null
        && y.Invariant.Description == null
        && !y.Invariant.FieldValues.Any()
        && !y.Locales.Any()),
      contentType, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new variant content.")]
  [InlineData(null)]
  [InlineData("b4ac888c-3d1e-4bfa-86f1-9cc08970130e")]
  public async Task Given_VariantNotExists_When_Handle_Then_ContentCreated(string? id)
  {
    Guid? contentId = id == null ? null : Guid.Parse(id);

    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    SelectProperties settings = new(isMultiple: false, options: [new SelectOption("MyCategory")]);
    FieldType articleCategory = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleCategory"), settings);
    FieldDefinition category = new(Guid.NewGuid(), articleCategory.Id, IsInvariant: true, IsRequired: true, IsIndexed: true, IsUnique: false,
      new Identifier("Category"), DisplayName: null, Description: null, Placeholder: null);

    FieldType articleTitle = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(minimumLength: 1, maximumLength: 100));
    FieldDefinition title = new(Guid.NewGuid(), articleTitle.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new Identifier("Title"), DisplayName: null, Description: null, Placeholder: null);
    contentType.SetField(title);

    Language language = new(new Locale("en"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    ContentModel content = new();
    _contentQuerier.Setup(x => x.ReadAsync(It.IsAny<Content>(), _cancellationToken)).ReturnsAsync(content);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.EntityId,
      UniqueName = "my-blog-article",
      DisplayName = " My Blog Article! ",
      Description = "  This is my first blog article.  ",
      FieldValues =
      [
        new FieldValue(Guid.Empty, bool.TrueString),
        new FieldValue(category.Id, "MyCategory"),
        new FieldValue(title.Id, " My Blog Article! ")
      ]
    };
    CreateOrReplaceContentCommand command = new(contentId, language.EntityId, payload);
    CreateOrReplaceContentResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.Content);
    Assert.Same(content, result.Content);

    _contentManager.Verify(x => x.SaveAsync(
      It.Is<Content>(y => y.RealmId == null
        && (!contentId.HasValue || contentId.Value == y.EntityId)
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.ContentTypeId == contentType.Id
        && y.Invariant.UniqueName.Value == payload.UniqueName
        && y.Invariant.DisplayName != null && y.Invariant.DisplayName.Value == payload.DisplayName.Trim()
        && y.Invariant.Description != null && y.Invariant.Description.Value == payload.Description.Trim()
        && y.Invariant.FieldValues.Count == 2
        && y.Invariant.FieldValues[Guid.Empty] == bool.TrueString
        && y.Invariant.FieldValues[category.Id] == "MyCategory"
        && y.Locales.Count == 1
        && y.Locales[language.Id].UniqueName.Value == payload.UniqueName
        && y.Locales[language.Id].DisplayName != null
        && y.Locales[language.Id].Description != null
        && y.Locales[language.Id].FieldValues.Count == 1
        && y.Locales[language.Id].FieldValues[title.Id] == "My Blog Article!"),
      contentType, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create/replace a content variant.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_VariantExists_When_Handle_Then_LocaleCreatedOrReplaced(bool exists)
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);
    _contentTypeRepository.Setup(x => x.LoadAsync(content, _cancellationToken)).ReturnsAsync(contentType);

    Language language = new(new Locale("en"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    Guid fieldId = Guid.NewGuid();
    if (exists)
    {
      ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues: new Dictionary<Guid, string>
      {
        [fieldId] = "749"
      });
      content.SetLocale(language, locale);
    }

    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(content, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentPayload payload = new()
    {
      UniqueName = "my-new-article",
      DisplayName = " My New Article! ",
      Description = "  This is my new blog article.  ",
      FieldValues =
      [
        new FieldValue(Guid.Empty, Guid.NewGuid().ToString()),
        new FieldValue(fieldId, " 1023 ")
      ]
    };
    CreateOrReplaceContentCommand command = new(content.EntityId, language.EntityId, payload);
    CreateOrReplaceContentResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Content);
    Assert.Same(model, result.Content);

    _contentManager.Verify(x => x.SaveAsync(content, contentType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, content.UpdatedBy);
    Assert.Equal(invariant, content.Invariant);

    KeyValuePair<LanguageId, ContentLocale> pair = Assert.Single(content.Locales);
    Assert.Equal(language.Id, pair.Key);
    Assert.Equal(payload.UniqueName, pair.Value.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), pair.Value.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), pair.Value.Description?.Value);

    Assert.Equal(payload.FieldValues.Count, pair.Value.FieldValues.Count);
    foreach (FieldValue field in payload.FieldValues)
    {
      Assert.Contains(pair.Value.FieldValues, f => f.Key == field.Id && f.Value == f.Value.Trim());
    }
  }

  [Fact(DisplayName = "It should replace an existing content invariant.")]
  public async Task Given_InvariantExists_When_Handle_Then_ContentReplaced()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"));
    Guid fieldId = Guid.NewGuid();
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, _faker.Internet.UserName()), displayName: null, description: null, fieldValues: new Dictionary<Guid, string>
    {
      [fieldId] = _faker.Person.FirstName
    });
    Content content = new(contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);
    _contentTypeRepository.Setup(x => x.LoadAsync(content, _cancellationToken)).ReturnsAsync(contentType);

    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(content, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceContentPayload payload = new()
    {
      UniqueName = _faker.Person.UserName,
      DisplayName = $" {_faker.Person.FullName} ",
      Description = "  I am the best author of this blog!  ",
      FieldValues =
      [
        new FieldValue(Guid.Empty, _faker.Person.Gender.ToString().ToLowerInvariant()),
        new FieldValue(fieldId, _faker.Person.FullName)
      ]
    };
    CreateOrReplaceContentCommand command = new(content.EntityId, LanguageId: null, payload);
    CreateOrReplaceContentResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Content);
    Assert.Same(model, result.Content);

    _contentManager.Verify(x => x.SaveAsync(content, contentType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, content.UpdatedBy);
    Assert.Equal(payload.UniqueName, content.Invariant.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), content.Invariant.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), content.Invariant.Description?.Value);

    Assert.Equal(payload.FieldValues.Count, content.Invariant.FieldValues.Count);
    foreach (FieldValue field in payload.FieldValues)
    {
      Assert.Contains(content.Invariant.FieldValues, f => f.Key == field.Id && f.Value == field.Value);
    }
  }

  [Fact(DisplayName = "It should throw ContentTypeNotFoundException when the content type could not be found.")]
  public async Task Given_ContentTypeNotFound_When_Handle_Then_ContentTypeNotFoundException()
  {
    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = Guid.NewGuid(),
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<ContentTypeNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.ContentTypeId.Value, exception.ContentTypeId);
    Assert.Equal("ContentTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguageNotFoundException when creating content and the language could not be found.")]
  public async Task Given_NotExistsLanguageNotFound_When_Handle_Then_LanguageNotFoundException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.EntityId,
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<LanguageNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.True(command.LanguageId.HasValue);
    Assert.Equal(command.LanguageId.Value, exception.LanguageId);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguageNotFoundException when replacing content and the language could not be found.")]
  public async Task Given_ExistsLanguageNotFound_When_Handle_Then_LanguageNotFoundException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);
    _contentTypeRepository.Setup(x => x.LoadAsync(content, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.EntityId,
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(content.EntityId, LanguageId: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<LanguageNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.True(command.LanguageId.HasValue);
    Assert.Equal(command.LanguageId.Value, exception.LanguageId);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when creating content and the content type ID is null.")]
  public async Task Given_NotExistsContentTypeIdNull_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceContentPayload payload = new()
    {
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("RequiredValidator", error.ErrorCode);
    Assert.Equal("ContentTypeId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when creating invariant content in a variant content type.")]
  public async Task Given_VariantWithoutLanguageId_When_Handle_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.EntityId,
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("InvariantValidator", error.ErrorCode);
    Assert.Equal("'LanguageId' cannot be null. The content type is not invariant.", error.ErrorMessage);
    Assert.Equal("LanguageId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when creating variant content in an invariant content type.")]
  public async Task Given_NotExistsInvariantWithLanguageId_When_Handle_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"), isInvariant: true);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.EntityId,
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("InvariantValidator", error.ErrorCode);
    Assert.Equal("'LanguageId' must be null. The content type is invariant.", error.ErrorMessage);
    Assert.Equal("LanguageId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when replacing variant content in an invariant content type.")]
  public async Task Given_ExistsInvariantWithLanguageId_When_Handle_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"), isInvariant: true);
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);
    _contentTypeRepository.Setup(x => x.LoadAsync(content, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.EntityId,
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(content.EntityId, LanguageId: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("InvariantValidator", error.ErrorCode);
    Assert.Equal("'LanguageId' must be null. The content type is invariant.", error.ErrorMessage);
    Assert.Equal("LanguageId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceContentPayload payload = new()
    {
      UniqueName = new("MyBlogArticle!"),
      DisplayName = RandomStringGenerator.GetString(999)
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
  }
}
