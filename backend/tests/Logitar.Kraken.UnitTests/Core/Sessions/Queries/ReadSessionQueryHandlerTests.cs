using Logitar.Kraken.Contracts.Sessions;
using Moq;

namespace Logitar.Kraken.Core.Sessions.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadSessionQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionQuerier> _sessionQuerier = new();

  private readonly ReadSessionQueryHandler _handler;

  public ReadSessionQueryHandlerTests()
  {
    _handler = new(_sessionQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when the session could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    Assert.Null(await _handler.Handle(new ReadSessionQuery(Guid.NewGuid()), _cancellationToken));
  }

  [Fact(DisplayName = "It should return the session found by ID.")]
  public async Task Given_Found_When_Handle_Then_SessionReturned()
  {
    SessionModel session = new()
    {
      Id = Guid.NewGuid()
    };
    _sessionQuerier.Setup(x => x.ReadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    ReadSessionQuery query = new(session.Id);
    SessionModel? result = await _handler.Handle(query, _cancellationToken);

    Assert.NotNull(result);
    Assert.Same(session, result);
  }
}
