using Bogus;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Settings;
using Moq;

namespace Logitar.Kraken.Core.Users.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadUserQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();

  private readonly ReadUserQueryHandler _handler;

  private readonly UserModel _user1 = new();
  private readonly UserModel _user2 = new();
  private readonly UserModel _user3 = new();

  public ReadUserQueryHandlerTests()
  {
    _handler = new(_applicationContext.Object, _userQuerier.Object);

    _user1 = new()
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Person.UserName
    };
    _user2 = new()
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Person.UserName,
      Email = new EmailModel(_faker.Person.Email)
    };
    _user3 = new()
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Internet.UserName(),
    };
    _user3.CustomIdentifiers.Add(new CustomIdentifierModel("Google", "1234567890"));

    UserSettings userSettings = new(new UniqueNameSettings(), new PasswordSettings(), requireUniqueEmail: true, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);
  }

  [Fact(DisplayName = "It should return null when no user was found.")]
  public async Task Given_NoUserFound_When_Handle_Then_NullReturned()
  {
    UserSettings userSettings = new(new UniqueNameSettings(), new PasswordSettings(), requireUniqueEmail: false, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    ReadUserQuery query = new(_user1.Id, _user2.UniqueName, _user3.CustomIdentifiers.Single());
    UserModel? user = await _handler.Handle(query, _cancellationToken);
    Assert.Null(user);

    _userQuerier.Verify(x => x.ReadAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "It should return the user found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_UserReturned()
  {
    _userQuerier.Setup(x => x.ReadAsync(_user1.Id, _cancellationToken)).ReturnsAsync(_user1);

    ReadUserQuery query = new(_user1.Id, UniqueName: null, _user3.CustomIdentifiers.Single());
    UserModel? user = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(user);
    Assert.Same(_user1, user);
  }

  [Fact(DisplayName = "It should return the user found by email address.")]
  public async Task Given_FoundByEmailAddress_When_Handle_Then_UserReturned()
  {
    Assert.NotNull(_user2.Email);
    _userQuerier.Setup(x => x.ReadAsync(_user2.Email, _cancellationToken)).ReturnsAsync([_user2]);

    ReadUserQuery query = new(Id: null, _user2.Email.Address, _user3.CustomIdentifiers.Single());
    UserModel? user = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(user);
    Assert.Same(_user2, user);
  }

  [Fact(DisplayName = "It should return the user found by unique name.")]
  public async Task Given_FoundByUniqueName_When_Handle_Then_UserReturned()
  {
    _userQuerier.Setup(x => x.ReadAsync(_user2.UniqueName, _cancellationToken)).ReturnsAsync(_user2);

    ReadUserQuery query = new(Id: null, _user2.UniqueName, _user3.CustomIdentifiers.Single());
    UserModel? user = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(user);
    Assert.Same(_user2, user);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many users were found.")]
  public async Task Given_ManyUsersFound_When_Handle_Then_TooManyResultsException()
  {
    _userQuerier.Setup(x => x.ReadAsync(_user1.Id, _cancellationToken)).ReturnsAsync(_user1);
    _userQuerier.Setup(x => x.ReadAsync(_user2.UniqueName, _cancellationToken)).ReturnsAsync(_user2);
    _userQuerier.Setup(x => x.ReadAsync(_user3.CustomIdentifiers.Single(), _cancellationToken)).ReturnsAsync(_user3);

    ReadUserQuery query = new(_user1.Id, _user2.UniqueName, _user3.CustomIdentifiers.Single());
    var exception = await Assert.ThrowsAsync<TooManyResultsException<UserModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }
}
