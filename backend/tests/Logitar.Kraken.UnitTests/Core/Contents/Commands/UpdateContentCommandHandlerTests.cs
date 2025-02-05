using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Localization;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateContentCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentManager> _contentManager = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly UpdateContentCommandHandler _handler;

  private readonly ContentType _contentType = new(new Identifier("BlogArticle"), isInvariant: false);

  public UpdateContentCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentManager.Object, _contentQuerier.Object, _contentRepository.Object, _languageRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Fact(DisplayName = "It should return null when the content was not found.")]
  public async Task Given_ContentNotFound_When_Handle_Then_NullReturned()
  {
    UpdateContentPayload payload = new();
    UpdateContentCommand command = new(Guid.NewGuid(), LanguageId: null, payload);
    ContentModel? content = await _handler.Handle(command, _cancellationToken);
    Assert.Null(content);
  }

  [Fact(DisplayName = "It should return null when the content locale was not found.")]
  public async Task Given_ContentLocaleNotFound_When_Handle_Then_NullReturned()
  {
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(_contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);

    Language language = new(new Locale("en"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    UpdateContentPayload payload = new();
    UpdateContentCommand command = new(content.EntityId, language.EntityId, payload);
    ContentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw LanguageNotFoundException when the language could not be found.")]
  public async Task Given_LanguageNotFound_When_Handle_Then_LanguageNotFoundException()
  {
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(_contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);

    UpdateContentPayload payload = new();
    UpdateContentCommand command = new(content.EntityId, Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<LanguageNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.True(command.LanguageId.HasValue);
    Assert.Equal(command.LanguageId.Value.ToString(), exception.Language);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    UpdateContentPayload payload = new()
    {
      UniqueName = "MyBlogArticle!",
      DisplayName = new ChangeModel<string>(RandomStringGenerator.GetString(999))
    };
    UpdateContentCommand command = new(Guid.NewGuid(), LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName.Value");
  }

  [Fact(DisplayName = "It should update an existing content invariant.")]
  public async Task Given_InvariantExists_When_Handle_Then_Updated()
  {
    Guid authorId = Guid.NewGuid();
    Guid wordCount = Guid.NewGuid();
    string authorIdValue = Guid.NewGuid().ToString();
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"), fieldValues: new Dictionary<Guid, string>
    {
      [Guid.Empty] = "empty",
      [authorId] = authorIdValue,
      [wordCount] = "749"
    });
    Content content = new(_contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);

    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(content, _cancellationToken)).ReturnsAsync(model);

    Guid publishedOn = Guid.NewGuid();
    string publishedOnValue = DateTime.Now.ToISOString();
    UpdateContentPayload payload = new()
    {
      DisplayName = new ChangeModel<string>(" My Blog Article! "),
      Description = new ChangeModel<string>("  This is my first blog article.  "),
      FieldValues =
      [
        new FieldValueUpdate(Guid.Empty, "    "),
        new FieldValueUpdate(wordCount, "1023"),
        new FieldValueUpdate(publishedOn, publishedOnValue)
      ]
    };
    UpdateContentCommand command = new(content.EntityId, LanguageId: null, payload);
    ContentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _contentManager.Verify(x => x.SaveAsync(content, _cancellationToken), Times.Once);

    Assert.Equal(invariant.UniqueName, content.Invariant.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), content.Invariant.DisplayName?.Value);
    Assert.Equal(payload.Description.Value?.Trim(), content.Invariant.Description?.Value);

    Assert.Equal(3, content.Invariant.FieldValues.Count);
    Assert.Contains(content.Invariant.FieldValues, f => f.Key == authorId && f.Value == authorIdValue);
    Assert.Contains(content.Invariant.FieldValues, f => f.Key == wordCount && f.Value == "1023");
    Assert.Contains(content.Invariant.FieldValues, f => f.Key == publishedOn && f.Value == publishedOnValue);
  }

  [Fact(DisplayName = "It should update an existing content locale.")]
  public async Task Given_VariantExists_When_Handle_Then_Updated()
  {
    Guid authorId = Guid.NewGuid();
    Guid wordCount = Guid.NewGuid();
    string authorIdValue = Guid.NewGuid().ToString();
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"), fieldValues: new Dictionary<Guid, string>
    {
      [Guid.Empty] = "empty",
      [authorId] = authorIdValue,
      [wordCount] = "749"
    });
    Content content = new(_contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);

    Language language = new(new Locale("en"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    content.SetLocale(language, invariant);

    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(content, _cancellationToken)).ReturnsAsync(model);

    Guid publishedOn = Guid.NewGuid();
    string publishedOnValue = DateTime.Now.ToISOString();
    UpdateContentPayload payload = new()
    {
      DisplayName = new ChangeModel<string>(" My Blog Article! "),
      Description = new ChangeModel<string>("  This is my first blog article.  "),
      FieldValues =
      [
        new FieldValueUpdate(Guid.Empty, "    "),
        new FieldValueUpdate(wordCount, "1023"),
        new FieldValueUpdate(publishedOn, publishedOnValue)
      ]
    };
    UpdateContentCommand command = new(content.EntityId, language.EntityId, payload);
    ContentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _contentManager.Verify(x => x.SaveAsync(content, _cancellationToken), Times.Once);

    KeyValuePair<LanguageId, ContentLocale> pair = Assert.Single(content.Locales);
    Assert.Equal(language.Id, pair.Key);
    ContentLocale locale = pair.Value;

    Assert.Equal(invariant.UniqueName, locale.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), locale.DisplayName?.Value);
    Assert.Equal(payload.Description.Value?.Trim(), locale.Description?.Value);

    Assert.Equal(3, locale.FieldValues.Count);
    Assert.Contains(locale.FieldValues, f => f.Key == authorId && f.Value == authorIdValue);
    Assert.Contains(locale.FieldValues, f => f.Key == wordCount && f.Value == "1023");
    Assert.Contains(locale.FieldValues, f => f.Key == publishedOn && f.Value == publishedOnValue);
  }
}
