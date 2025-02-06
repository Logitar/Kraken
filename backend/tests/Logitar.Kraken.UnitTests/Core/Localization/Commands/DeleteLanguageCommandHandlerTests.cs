using Bogus;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Dictionaries.Events;
using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteLanguageCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IDictionaryRepository> _dictionaryRepository = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly DeleteLanguageCommandHandler _handler;

  private readonly Language _language;

  public DeleteLanguageCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentRepository.Object, _dictionaryRepository.Object, _languageQuerier.Object, _languageRepository.Object);

    _applicationContext.SetupGet(x => x.ActorId).Returns(_actorId);

    _language = new(new Locale("en"), isDefault: true, actorId: null, LanguageId.NewId(_realmId));
    _languageRepository.Setup(x => x.LoadAsync(_language.Id, _cancellationToken)).ReturnsAsync(_language);
  }

  [Fact(DisplayName = "It should delete an existing language.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    Dictionary dictionary = new(_language, actorId: null, DictionaryId.NewId(_realmId));
    _dictionaryRepository.Setup(x => x.LoadAsync(_language.Id, _cancellationToken)).ReturnsAsync(dictionary);

    ContentType contentType = new(new Identifier("Person"));
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName));
    Content content = new(contentType, invariant, actorId: null, ContentId.NewId(_realmId));
    content.SetLocale(_language, invariant);
    _contentRepository.Setup(x => x.LoadAsync(_language.Id, _cancellationToken)).ReturnsAsync([content]);

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(_language, _cancellationToken)).ReturnsAsync(model);

    DeleteLanguageCommand command = new(_language.EntityId);
    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Contains(content.Changes, change => change is ContentLocaleRemoved removed && removed.ActorId == _actorId && removed.LanguageId == _language.Id);
    Assert.Contains(dictionary.Changes, change => change is DictionaryDeleted deleted && deleted.ActorId == _actorId);
    Assert.True(_language.IsDeleted);

    _languageRepository.Verify(x => x.SaveAsync(_language, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the language could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteLanguageCommand command = new(_language.EntityId);
    LanguageModel? language = await _handler.Handle(command, _cancellationToken);
    Assert.Null(language);
  }
}
