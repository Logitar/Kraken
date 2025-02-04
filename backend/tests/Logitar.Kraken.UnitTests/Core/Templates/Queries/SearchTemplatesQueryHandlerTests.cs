using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Templates;
using Moq;

namespace Logitar.Kraken.Core.Templates.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchTemplatesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ITemplateQuerier> _templateQuerier = new();

  private readonly SearchTemplatesQueryHandler _handler;

  public SearchTemplatesQueryHandlerTests()
  {
    _handler = new(_templateQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchTemplatesPayload payload = new();
    SearchResults<TemplateModel> templates = new();
    _templateQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(templates);

    SearchTemplatesQuery query = new(payload);
    SearchResults<TemplateModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(templates, results);
  }
}
