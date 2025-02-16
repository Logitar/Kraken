using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Contracts.Localization;
using Moq;

namespace Logitar.Kraken.Core.Dictionaries.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadDictionaryQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IDictionaryQuerier> _dictionaryQuerier = new();

  private readonly ReadDictionaryQueryHandler _handler;

  private readonly DictionaryModel _english = new()
  {
    Id = Guid.NewGuid(),
    Language = new LanguageModel
    {
      IsDefault = true,
      Locale = new LocaleModel("en")
    }
  };
  private readonly DictionaryModel _french = new()
  {
    Id = Guid.NewGuid(),
    Language = new LanguageModel
    {
      Locale = new LocaleModel("fr")
    }
  };

  public ReadDictionaryQueryHandlerTests()
  {
    _handler = new(_dictionaryQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when no dictionary was found.")]
  public async Task Given_NoDictionaryFound_When_Handle_Then_NullReturned()
  {
    ReadDictionaryQuery query = new(_english.Id, _french.Language.Locale.Code);
    DictionaryModel? dictionary = await _handler.Handle(query, _cancellationToken);
    Assert.Null(dictionary);
  }

  [Fact(DisplayName = "It should return the dictionary found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_DictionaryReturned()
  {
    _dictionaryQuerier.Setup(x => x.ReadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);

    ReadDictionaryQuery query = new(_english.Id, Language: null);
    DictionaryModel? dictionary = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(dictionary);
    Assert.Same(_english, dictionary);
  }

  [Fact(DisplayName = "It should return the dictionary found by unique name.")]
  public async Task Given_FoundByUniqueKey_When_Handle_Then_DictionaryReturned()
  {
    _dictionaryQuerier.Setup(x => x.ReadAsync(_french.Language.Locale.Code, _cancellationToken)).ReturnsAsync(_french);

    ReadDictionaryQuery query = new(Id: null, _french.Language.Locale.Code);
    DictionaryModel? dictionary = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(dictionary);
    Assert.Same(_french, dictionary);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many dictionaries were found.")]
  public async Task Given_ManyDictionariesFound_When_Handle_Then_TooManyResultsException()
  {
    _dictionaryQuerier.Setup(x => x.ReadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);
    _dictionaryQuerier.Setup(x => x.ReadAsync(_french.Language.Locale.Code, _cancellationToken)).ReturnsAsync(_french);

    ReadDictionaryQuery query = new(_english.Id, _french.Language.Locale.Code);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<DictionaryModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
