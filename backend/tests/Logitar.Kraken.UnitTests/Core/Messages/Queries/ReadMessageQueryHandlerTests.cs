using Logitar.Kraken.Contracts.Messages;
using Moq;

namespace Logitar.Kraken.Core.Messages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadMessageQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IMessageQuerier> _messageQuerier = new();

  private readonly ReadMessageQueryHandler _handler;

  public ReadMessageQueryHandlerTests()
  {
    _handler = new(_messageQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when the message could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    Assert.Null(await _handler.Handle(new ReadMessageQuery(Guid.NewGuid()), _cancellationToken));
  }

  [Fact(DisplayName = "It should return the message found by ID.")]
  public async Task Given_Found_When_Handle_Then_MessageReturned()
  {
    MessageModel message = new()
    {
      Id = Guid.NewGuid()
    };
    _messageQuerier.Setup(x => x.ReadAsync(message.Id, _cancellationToken)).ReturnsAsync(message);

    ReadMessageQuery query = new(message.Id);
    MessageModel? result = await _handler.Handle(query, _cancellationToken);

    Assert.NotNull(result);
    Assert.Same(message, result);
  }
}
