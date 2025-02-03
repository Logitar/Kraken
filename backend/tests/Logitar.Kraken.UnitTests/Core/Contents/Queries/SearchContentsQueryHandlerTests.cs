using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchContentsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentQuerier> _contentQuerier = new();

  private readonly SearchContentsQueryHandler _handler;

  public SearchContentsQueryHandlerTests()
  {
    _handler = new(_contentQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchContentsPayload payload = new();
    SearchResults<ContentLocaleModel> contents = new();
    _contentQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(contents);

    SearchContentsQuery query = new(payload);
    SearchResults<ContentLocaleModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(contents, results);
  }
}
