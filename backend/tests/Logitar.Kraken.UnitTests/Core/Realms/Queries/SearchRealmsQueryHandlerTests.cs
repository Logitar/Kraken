using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.Realms.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchRealmsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRealmQuerier> _realmQuerier = new();

  private readonly SearchRealmsQueryHandler _handler;

  public SearchRealmsQueryHandlerTests()
  {
    _handler = new(_realmQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchRealmsPayload payload = new();
    SearchResults<RealmModel> realms = new();
    _realmQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(realms);

    SearchRealmsQuery query = new(payload);
    SearchResults<RealmModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(realms, results);
  }
}
