using Logitar.Kraken.Contracts.Localization;
using Moq;

namespace Logitar.Kraken.Core.Localization.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadLanguageQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();

  private readonly ReadLanguageQueryHandler _handler;

  private readonly LanguageModel _english = new(new LocaleModel("en"))
  {
    Id = Guid.NewGuid(),
    IsDefault = true
  };
  private readonly LanguageModel _french = new(new LocaleModel("fr"))
  {
    Id = Guid.NewGuid()
  };
  private readonly LanguageModel _spanish = new(new LocaleModel("es"))
  {
    Id = Guid.NewGuid()
  };

  public ReadLanguageQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object);
  }

  [Fact(DisplayName = "Handle: it should return null when language was found.")]
  public async Task Given_NoLanguageFound_When_Handle_Then_NullReturned()
  {
    ReadLanguageQuery query = new(_french.Id, _spanish.Locale.Code, IsDefault: false);
    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.Null(language);
  }

  [Fact(DisplayName = "Handle: it should return the default language.")]
  public async Task Given_Default_When_Handle_Then_LanguageReturned()
  {
    _languageQuerier.Setup(x => x.ReadDefaultAsync(_cancellationToken)).ReturnsAsync(_english);

    ReadLanguageQuery query = new(Id: null, Locale: null, IsDefault: true);
    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(language);
    Assert.Same(_english, language);
  }

  [Fact(DisplayName = "Handle: it should return the language found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_LanguageReturned()
  {
    _languageQuerier.Setup(x => x.ReadAsync(_spanish.Id, _cancellationToken)).ReturnsAsync(_spanish);

    ReadLanguageQuery query = new(_spanish.Id, Locale: null, IsDefault: false);
    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(language);
    Assert.Same(_spanish, language);
  }

  [Fact(DisplayName = "Handle: it should return the language found by locale.")]
  public async Task Given_FoundByLocale_When_Handle_Then_LanguageReturned()
  {
    _languageQuerier.Setup(x => x.ReadAsync(_french.Locale.Code, _cancellationToken)).ReturnsAsync(_french);

    ReadLanguageQuery query = new(Id: null, _french.Locale.Code, IsDefault: false);
    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(language);
    Assert.Same(_french, language);
  }

  [Fact(DisplayName = "Handle: it should throw TooManyResultsException when many languages were found.")]
  public async Task Given_ManyLanguagesFound_When_Handle_Then_TooManyResultsException()
  {
    _languageQuerier.Setup(x => x.ReadAsync(_spanish.Id, _cancellationToken)).ReturnsAsync(_spanish);
    _languageQuerier.Setup(x => x.ReadAsync(_french.Locale.Code, _cancellationToken)).ReturnsAsync(_french);
    _languageQuerier.Setup(x => x.ReadDefaultAsync(_cancellationToken)).ReturnsAsync(_english);

    ReadLanguageQuery query = new(_spanish.Id, _french.Locale.Code, IsDefault: true);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<LanguageModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }
}
