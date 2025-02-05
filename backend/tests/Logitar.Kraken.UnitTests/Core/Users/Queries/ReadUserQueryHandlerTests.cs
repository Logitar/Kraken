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

  private readonly UserSettings _userSettings = new(new UniqueNameSettings(), new PasswordSettings(), RequireUniqueEmail: false, RequireConfirmedAccount: false);
  private readonly UserModel _user1;
  private readonly UserModel _user2;
  private readonly UserModel _user3;

  public ReadUserQueryHandlerTests()
  {
    _handler = new(_applicationContext.Object, _userQuerier.Object);

    _user1 = new UserModel
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Person.UserName
    };
    _user2 = new UserModel
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Internet.UserName()
    };
    _user3 = new UserModel
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Internet.UserName(),
      CustomIdentifiers = [new CustomIdentifierModel("Google", _faker.Random.String(10, minChar: '0', maxChar: '9'))]
    };

    _applicationContext.SetupGet(x => x.UserSettings).Returns(_userSettings);
  }

  [Fact(DisplayName = "It should return null when no user was found.")]
  public async Task Given_NoUserFound_When_Handle_Then_NullReturned()
  {
    ReadUserQuery query = new(_user1.Id, _user2.UniqueName, _user3.CustomIdentifiers.Single());
    UserModel? user = await _handler.Handle(query, _cancellationToken);
    Assert.Null(user);

    _userQuerier.Verify(x => x.ReadAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "It should return the user found by custom identifier.")]
  public async Task Given_FoundByCustomIdentifier_When_Handle_Then_UserReturned()
  {
    CustomIdentifierModel customIdentifier = Assert.Single(_user3.CustomIdentifiers);
    _userQuerier.Setup(x => x.ReadAsync(customIdentifier, _cancellationToken)).ReturnsAsync(_user3);

    ReadUserQuery query = new(Id: null, UniqueName: null, customIdentifier);
    UserModel? user = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(user);
    Assert.Same(_user3, user);
  }

  [Fact(DisplayName = "It should return the user found by email address.")]
  public async Task Given_FoundByEmailAddress_When_Handle_Then_UserReturned()
  {
    UserSettings userSettings = new(_userSettings.UniqueName, _userSettings.Password, RequireUniqueEmail: true, _userSettings.RequireConfirmedAccount);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    UserModel user = new()
    {
      Email = new EmailModel(_faker.Person.Email)
      {
        IsVerified = true
      }
    };
    _userQuerier.Setup(x => x.ReadAsync(It.Is<IEmail>(email => email.Address == user.Email.Address), _cancellationToken)).ReturnsAsync([user]);

    ReadUserQuery query = new(Id: null, user.Email.Address, Identifier: null);
    UserModel? found = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(found);
    Assert.Same(user, found);
  }

  [Fact(DisplayName = "It should return the user found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_UserReturned()
  {
    _userQuerier.Setup(x => x.ReadAsync(_user1.Id, _cancellationToken)).ReturnsAsync(_user1);

    ReadUserQuery query = new(_user1.Id, UniqueName: null, Identifier: null);
    UserModel? user = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(user);
    Assert.Same(_user1, user);
  }

  [Fact(DisplayName = "It should return the user found by unique name.")]
  public async Task Given_FoundByUniqueKey_When_Handle_Then_UserReturned()
  {
    _userQuerier.Setup(x => x.ReadAsync(_user2.UniqueName, _cancellationToken)).ReturnsAsync(_user2);

    ReadUserQuery query = new(Id: null, _user2.UniqueName, Identifier: null);
    UserModel? user = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(user);
    Assert.Same(_user2, user);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many users were found.")]
  public async Task Given_ManyUsersFound_When_Handle_Then_TooManyResultsException()
  {
    _userQuerier.Setup(x => x.ReadAsync(_user1.Id, _cancellationToken)).ReturnsAsync(_user1);
    _userQuerier.Setup(x => x.ReadAsync(_user2.UniqueName, _cancellationToken)).ReturnsAsync(_user2);

    CustomIdentifierModel customIdentifier = Assert.Single(_user3.CustomIdentifiers);
    _userQuerier.Setup(x => x.ReadAsync(customIdentifier, _cancellationToken)).ReturnsAsync(_user3);

    ReadUserQuery query = new(_user1.Id, _user2.UniqueName, customIdentifier);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<UserModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }
}
