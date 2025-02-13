using Bogus;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.Core.ApiKeys.Events;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.Core.Users.Events;
using Moq;

namespace Logitar.Kraken.Core.Roles.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteRoleCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApiKeyRepository> _apiKeyRepository = new();
  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IRoleQuerier> _roleQuerier = new();
  private readonly Mock<IRoleRepository> _roleRepository = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly DeleteRoleCommandHandler _handler;

  private readonly Role _role;

  public DeleteRoleCommandHandlerTests()
  {
    _handler = new(_apiKeyRepository.Object, _applicationContext.Object, _roleQuerier.Object, _roleRepository.Object, _userRepository.Object);

    _role = new(new UniqueName(new UniqueNameSettings(), "admin"), actorId: null, RoleId.NewId(_realmId));
    _roleRepository.Setup(x => x.LoadAsync(_role.Id, _cancellationToken)).ReturnsAsync(_role);
  }

  [Fact(DisplayName = "It should delete an existing role.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    RoleModel model = new();
    _roleQuerier.Setup(x => x.ReadAsync(_role, _cancellationToken)).ReturnsAsync(model);

    Base64Password secret = new(_faker.Random.String(6, minChar: '0', maxChar: '9'));
    ApiKey[] apiKeys = [new(secret, new DisplayName("Test"), apiKeyId: ApiKeyId.NewId(_realmId))];
    foreach (ApiKey apiKey in apiKeys)
    {
      apiKey.AddRole(_role);
    }
    _apiKeyRepository.Setup(x => x.LoadAsync(_role.Id, _cancellationToken)).ReturnsAsync(apiKeys);

    User[] users = [new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), userId: UserId.NewId(_realmId))];
    foreach (User user in users)
    {
      user.AddRole(_role);
    }
    _userRepository.Setup(x => x.LoadAsync(_role.Id, _cancellationToken)).ReturnsAsync(users);

    DeleteRoleCommand command = new(_role.EntityId);
    RoleModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(apiKeys.All(apiKey => apiKey.Changes.Any(change => change is ApiKeyRoleRemoved removed && removed.RoleId == _role.Id)));
    Assert.True(users.All(user => user.Changes.Any(change => change is UserRoleRemoved removed && removed.RoleId == _role.Id)));
    Assert.True(_role.IsDeleted);

    _apiKeyRepository.Verify(x => x.SaveAsync(apiKeys, _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.SaveAsync(users, _cancellationToken), Times.Once());
    _roleRepository.Verify(x => x.SaveAsync(_role, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the role could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteRoleCommand command = new(_role.EntityId);
    RoleModel? role = await _handler.Handle(command, _cancellationToken);
    Assert.Null(role);
  }
}
