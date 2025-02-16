using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Users;
using Moq;

namespace Logitar.Kraken.Core.Users.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchUsersQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IUserQuerier> _userQuerier = new();

  private readonly SearchUsersQueryHandler _handler;

  public SearchUsersQueryHandlerTests()
  {
    _handler = new(_userQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchUsersPayload payload = new();
    SearchResults<UserModel> users = new();
    _userQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(users);

    SearchUsersQuery query = new(payload);
    SearchResults<UserModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(users, results);
  }
}
