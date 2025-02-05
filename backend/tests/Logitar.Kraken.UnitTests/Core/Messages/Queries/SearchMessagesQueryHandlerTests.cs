using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.Messages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchMessagesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IMessageQuerier> _messageQuerier = new();

  private readonly SearchMessagesQueryHandler _handler;

  public SearchMessagesQueryHandlerTests()
  {
    _handler = new(_messageQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchMessagesPayload payload = new();
    SearchResults<MessageModel> messages = new();
    _messageQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(messages);

    SearchMessagesQuery query = new(payload);
    SearchResults<MessageModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(messages, results);
  }
}
