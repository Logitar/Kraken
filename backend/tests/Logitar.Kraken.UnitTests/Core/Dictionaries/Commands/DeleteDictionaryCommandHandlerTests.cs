using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.Dictionaries.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteDictionaryCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IDictionaryQuerier> _dictionaryQuerier = new();
  private readonly Mock<IDictionaryRepository> _dictionaryRepository = new();

  private readonly DeleteDictionaryCommandHandler _handler;

  private readonly Dictionary _dictionary;

  public DeleteDictionaryCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _dictionaryQuerier.Object, _dictionaryRepository.Object);

    Language language = new(new Locale("en"), languageId: LanguageId.NewId(_realmId));
    _dictionary = new(language, actorId: null, DictionaryId.NewId(_realmId));
    _dictionaryRepository.Setup(x => x.LoadAsync(_dictionary.Id, _cancellationToken)).ReturnsAsync(_dictionary);
  }

  [Fact(DisplayName = "It should delete an existing dictionary.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    DictionaryModel model = new();
    _dictionaryQuerier.Setup(x => x.ReadAsync(_dictionary, _cancellationToken)).ReturnsAsync(model);

    DeleteDictionaryCommand command = new(_dictionary.EntityId);
    DictionaryModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(_dictionary.IsDeleted);

    _dictionaryRepository.Verify(x => x.SaveAsync(_dictionary, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the dictionary could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteDictionaryCommand command = new(_dictionary.EntityId);
    DictionaryModel? dictionary = await _handler.Handle(command, _cancellationToken);
    Assert.Null(dictionary);
  }
}
