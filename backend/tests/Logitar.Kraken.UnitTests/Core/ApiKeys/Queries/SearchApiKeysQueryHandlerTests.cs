using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.ApiKeys.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchApiKeysQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApiKeyQuerier> _apikeyQuerier = new();

  private readonly SearchApiKeysQueryHandler _handler;

  public SearchApiKeysQueryHandlerTests()
  {
    _handler = new(_apikeyQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchApiKeysPayload payload = new();
    SearchResults<ApiKeyModel> apikeys = new();
    _apikeyQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(apikeys);

    SearchApiKeysQuery query = new(payload);
    SearchResults<ApiKeyModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(apikeys, results);
  }
}
