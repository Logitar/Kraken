using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Senders;
using Moq;

namespace Logitar.Kraken.Core.Senders.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchSendersQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISenderQuerier> _senderQuerier = new();

  private readonly SearchSendersQueryHandler _handler;

  public SearchSendersQueryHandlerTests()
  {
    _handler = new(_senderQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchSendersPayload payload = new();
    SearchResults<SenderModel> senders = new();
    _senderQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(senders);

    SearchSendersQuery query = new(payload);
    SearchResults<SenderModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(senders, results);
  }
}
