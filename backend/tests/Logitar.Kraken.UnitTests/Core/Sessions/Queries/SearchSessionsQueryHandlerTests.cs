using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Sessions;
using Moq;

namespace Logitar.Kraken.Core.Sessions.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchSessionsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionQuerier> _sessionQuerier = new();

  private readonly SearchSessionsQueryHandler _handler;

  public SearchSessionsQueryHandlerTests()
  {
    _handler = new(_sessionQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchSessionsPayload payload = new();
    SearchResults<SessionModel> sessions = new();
    _sessionQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(sessions);

    SearchSessionsQuery query = new(payload);
    SearchResults<SessionModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(sessions, results);
  }
}
