using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchPublishedContentsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPublishedContentQuerier> _publishedContentQuerier = new();

  private readonly SearchPublishedContentsQueryHandler _handler;

  public SearchPublishedContentsQueryHandlerTests()
  {
    _handler = new(_publishedContentQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchPublishedContentsPayload payload = new();
    SearchResults<PublishedContentLocale> publishedLocales = new();
    _publishedContentQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(publishedLocales);

    SearchPublishedContentsQuery query = new(payload);
    SearchResults<PublishedContentLocale> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(publishedLocales, results);
  }
}
