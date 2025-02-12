using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.Localization.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchLanguagesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();

  private readonly SearchLanguagesQueryHandler _handler;

  public SearchLanguagesQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object);
  }

  [Fact(DisplayName = "Handle: it should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchLanguagesPayload payload = new();
    SearchResults<LanguageModel> languages = new();
    _languageQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(languages);

    SearchLanguagesQuery query = new(payload);
    SearchResults<LanguageModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(languages, results);
  }
}
