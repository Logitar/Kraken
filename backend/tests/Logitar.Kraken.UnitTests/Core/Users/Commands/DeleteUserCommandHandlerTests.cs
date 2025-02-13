using Bogus;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Sessions;
using Logitar.Kraken.Core.Settings;
using Moq;

namespace Logitar.Kraken.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteUserCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IOneTimePasswordRepository> _oneTimePasswordRepository = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly DeleteUserCommandHandler _handler;

  private readonly User _user;

  public DeleteUserCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _oneTimePasswordRepository.Object, _sessionRepository.Object, _userQuerier.Object, _userRepository.Object);

    _user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), password: null, actorId: null, UserId.NewId(_realmId));
    _userRepository.Setup(x => x.LoadAsync(_user.Id, _cancellationToken)).ReturnsAsync(_user);
  }

  [Fact(DisplayName = "It should delete an existing user.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    UserModel model = new();
    _userQuerier.Setup(x => x.ReadAsync(_user, _cancellationToken)).ReturnsAsync(model);

    Base64Password password = new(_faker.Random.String(6, minChar: '0', maxChar: '9'));
    OneTimePassword[] oneTimePasswords = [new(password, _user, oneTimePasswordId: OneTimePasswordId.NewId(_realmId))];
    _oneTimePasswordRepository.Setup(x => x.LoadAsync(_user.Id, _cancellationToken)).ReturnsAsync(oneTimePasswords);

    Session[] sessions = [new(_user, sessionId: SessionId.NewId(_realmId))];
    _sessionRepository.Setup(x => x.LoadAsync(_user.Id, _cancellationToken)).ReturnsAsync(sessions);

    DeleteUserCommand command = new(_user.EntityId);
    UserModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(oneTimePasswords.All(oneTimePassword => oneTimePassword.IsDeleted));
    Assert.True(sessions.All(session => session.IsDeleted));
    Assert.True(_user.IsDeleted);

    _oneTimePasswordRepository.Verify(x => x.SaveAsync(oneTimePasswords, _cancellationToken), Times.Once());
    _sessionRepository.Verify(x => x.SaveAsync(sessions, _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.SaveAsync(_user, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the user could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteUserCommand command = new(_user.EntityId);
    UserModel? user = await _handler.Handle(command, _cancellationToken);
    Assert.Null(user);
  }
}
