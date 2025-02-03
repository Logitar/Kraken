using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchContentTypesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();

  private readonly SearchContentTypesQueryHandler _handler;

  public SearchContentTypesQueryHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchContentTypesPayload payload = new();
    SearchResults<ContentTypeModel> contentTypes = new();
    _contentTypeQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(contentTypes);

    SearchContentTypesQuery query = new(payload);
    SearchResults<ContentTypeModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(contentTypes, results);
  }
}
