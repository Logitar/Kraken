using Bogus;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Localization;
using Moq;

namespace Logitar.Kraken.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteContentCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new()
  {
    ReferenceHandler = ReferenceHandler.IgnoreCycles
  };

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<ILanguageManager> _languageManager = new();

  private readonly DeleteContentCommandHandler _handler;

  private readonly Language _english = new(new Locale("en"), isDefault: true);
  private readonly Language _french = new(new Locale("fr"));
  private readonly ContentType _contentType = new(new Identifier("Person"), isInvariant: false);
  private readonly Content _content;

  public DeleteContentCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentQuerier.Object, _contentRepository.Object, _languageManager.Object);

    _applicationContext.SetupGet(x => x.ActorId).Returns(_actorId);

    _content = new(_contentType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName)));
    _contentRepository.Setup(x => x.LoadAsync(_content.Id, _cancellationToken)).ReturnsAsync(_content);

    _languageManager.Setup(x => x.FindAsync(It.Is<string>(s => s.Trim().Equals(_english.EntityId.ToString(), StringComparison.InvariantCultureIgnoreCase)), _cancellationToken))
      .ReturnsAsync(_english);
    _languageManager.Setup(x => x.FindAsync(It.Is<string>(s => s.Trim().Equals(_english.Locale.Code, StringComparison.InvariantCultureIgnoreCase)), _cancellationToken))
      .ReturnsAsync(_english);
  }

  [Fact(DisplayName = "It should delete a content locale and remove it from the model.")]
  public async Task Given_Language_When_Handle_Then_LocaleDeletedRemoved()
  {
    _content.SetLocale(_english, _content.Invariant);
    _content.SetLocale(_french, _content.Invariant);

    ContentModel model = new()
    {
      Id = _content.EntityId
    };
    model.Invariant = new ContentLocaleModel(model)
    {
      UniqueName = _content.Invariant.UniqueName.Value
    };
    model.Locales.Add(new ContentLocaleModel(model)
    {
      Language = new LanguageModel(new LocaleModel(_english.Locale.Code), _english.IsDefault)
      {
        Id = _english.EntityId
      },
      UniqueName = _content.Invariant.UniqueName.Value
    });
    model.Locales.Add(new ContentLocaleModel(model)
    {
      Language = new LanguageModel(new LocaleModel(_french.Locale.Code), _french.IsDefault)
      {
        Id = _french.EntityId
      },
      UniqueName = _content.Invariant.UniqueName.Value
    });
    _contentQuerier.Setup(x => x.ReadAsync(_content, _cancellationToken)).ReturnsAsync(model);

    DeleteContentCommand command = new(_content.EntityId, $"  {_english.EntityId.ToString().ToUpperInvariant()}  ");
    ContentModel? content = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(content);
    Assert.Same(model, content);

    Assert.NotNull(content.Invariant);
    Assert.Equal(_french.EntityId, Assert.Single(content.Locales).Language?.Id);

    Assert.Equal(_french.Id, Assert.Single(_content.Locales).Key);
    Assert.Contains(_content.Changes, change => change is ContentLocaleRemoved deleted && deleted.LanguageId == _english.Id && deleted.ActorId == _actorId);

    _contentRepository.Verify(x => x.SaveAsync(_content, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should delete an existing content.")]
  public async Task Given_NoLanguage_When_Handle_Then_ContentDeleted()
  {
    _content.SetLocale(_english, _content.Invariant);

    ContentModel model = new()
    {
      Id = _content.EntityId
    };
    model.Invariant = new ContentLocaleModel(model)
    {
      UniqueName = _content.Invariant.UniqueName.Value
    };
    model.Locales.Add(new ContentLocaleModel(model)
    {
      Language = new LanguageModel(new LocaleModel(_english.Locale.Code), _english.IsDefault)
      {
        Id = _english.EntityId
      },
      UniqueName = _content.Invariant.UniqueName.Value
    });
    _contentQuerier.Setup(x => x.ReadAsync(_content, _cancellationToken)).ReturnsAsync(model);

    string json = JsonSerializer.Serialize(model, _serializerOptions);

    DeleteContentCommand command = new(_content.EntityId, Language: "    ");
    ContentModel? content = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(content);
    Assert.Equal(json, JsonSerializer.Serialize(content, _serializerOptions));

    Assert.True(_content.IsDeleted);
    Assert.Contains(_content.Changes, change => change is ContentDeleted deleted && deleted.ActorId == _actorId);

    _contentRepository.Verify(x => x.SaveAsync(_content, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the content could not be found.")]
  public async Task Given_ContentNotFound_When_Handle_Then_NullReturned()
  {
    DeleteContentCommand command = new(Guid.NewGuid(), Language: null);
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should return null when the locale could not be found.")]
  public async Task Given_LocaleNotFound_When_Handle_Then_NullReturned()
  {
    DeleteContentCommand command = new(_content.EntityId, Language: $"  {_english.Locale.Code.ToUpperInvariant()}  ");
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }
}
