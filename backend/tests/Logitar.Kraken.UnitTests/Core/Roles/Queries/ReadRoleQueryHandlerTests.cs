using Logitar.Kraken.Contracts.Roles;
using Moq;

namespace Logitar.Kraken.Core.Roles.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadRoleQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRoleQuerier> _roleQuerier = new();

  private readonly ReadRoleQueryHandler _handler;

  private readonly RoleModel _admin = new()
  {
    Id = Guid.NewGuid(),
    UniqueName = "the-old-world"
  };
  private readonly RoleModel _guest = new()
  {
    Id = Guid.NewGuid(),
    UniqueName = "the-new-world"
  };

  public ReadRoleQueryHandlerTests()
  {
    _handler = new(_roleQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when no role was found.")]
  public async Task Given_NoRoleFound_When_Handle_Then_NullReturned()
  {
    ReadRoleQuery query = new(_admin.Id, _guest.UniqueName);
    RoleModel? role = await _handler.Handle(query, _cancellationToken);
    Assert.Null(role);
  }

  [Fact(DisplayName = "It should return the role found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_RoleReturned()
  {
    _roleQuerier.Setup(x => x.ReadAsync(_admin.Id, _cancellationToken)).ReturnsAsync(_admin);

    ReadRoleQuery query = new(_admin.Id, UniqueName: null);
    RoleModel? role = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(role);
    Assert.Same(_admin, role);
  }

  [Fact(DisplayName = "It should return the role found by unique name.")]
  public async Task Given_FoundByUniqueName_When_Handle_Then_RoleReturned()
  {
    _roleQuerier.Setup(x => x.ReadAsync(_guest.UniqueName, _cancellationToken)).ReturnsAsync(_guest);

    ReadRoleQuery query = new(Id: null, _guest.UniqueName);
    RoleModel? role = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(role);
    Assert.Same(_guest, role);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many roles were found.")]
  public async Task Given_ManyRolesFound_When_Handle_Then_TooManyResultsException()
  {
    _roleQuerier.Setup(x => x.ReadAsync(_admin.Id, _cancellationToken)).ReturnsAsync(_admin);
    _roleQuerier.Setup(x => x.ReadAsync(_guest.UniqueName, _cancellationToken)).ReturnsAsync(_guest);

    ReadRoleQuery query = new(_admin.Id, _guest.UniqueName);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<RoleModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
