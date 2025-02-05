using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Search;
using Moq;

namespace Logitar.Kraken.Core.Roles.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchRolesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRoleQuerier> _roleQuerier = new();

  private readonly SearchRolesQueryHandler _handler;

  public SearchRolesQueryHandlerTests()
  {
    _handler = new(_roleQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchRolesPayload payload = new();
    SearchResults<RoleModel> roles = new();
    _roleQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(roles);

    SearchRolesQuery query = new(payload);
    SearchResults<RoleModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(roles, results);
  }
}
