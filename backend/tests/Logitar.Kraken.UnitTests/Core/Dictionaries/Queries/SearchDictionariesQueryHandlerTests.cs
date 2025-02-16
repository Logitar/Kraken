using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.Dictionaries.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchDictionariesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IDictionaryQuerier> _dictionaryQuerier = new();

  private readonly SearchDictionariesQueryHandler _handler;

  public SearchDictionariesQueryHandlerTests()
  {
    _handler = new(_dictionaryQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchDictionariesPayload payload = new();
    SearchResults<DictionaryModel> dictionaries = new();
    _dictionaryQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(dictionaries);

    SearchDictionariesQuery query = new(payload);
    SearchResults<DictionaryModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(dictionaries, results);
  }
}
