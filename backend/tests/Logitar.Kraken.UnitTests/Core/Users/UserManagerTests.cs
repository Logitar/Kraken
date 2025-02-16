using Bogus;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class UserManagerTests
{
  private const string PropertyName = "User";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly UserManager _manager;

  private readonly User _user;
  private readonly UserSettings _userSettings = new(new UniqueNameSettings(), new PasswordSettings(), requireUniqueEmail: false, requireConfirmedAccount: false);

  public UserManagerTests()
  {
    _manager = new(_applicationContext.Object, _userQuerier.Object, _userRepository.Object);

    _user = new(new UniqueName(_userSettings.UniqueName, _faker.Person.UserName));

    _applicationContext.SetupGet(x => x.UserSettings).Returns(_userSettings);
  }

  [Fact(DisplayName = "FindAsync: it should return the user found by custom identifier.")]
  public async Task Given_CustomIdentifier_When_FindAsync_Then_Found()
  {
    UserSettings userSettings = new(_userSettings.UniqueName, _userSettings.Password, requireUniqueEmail: true, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    Identifier key = new("Google");
    CustomIdentifier value = new(_faker.Random.String(length: 16, minChar: '0', maxChar: '9'));
    _userRepository.Setup(x => x.LoadAsync(null, key, value, _cancellationToken)).ReturnsAsync(_user);

    User found = await _manager.FindAsync($"  {key}:{value}  ", PropertyName, includeId: true, _cancellationToken);
    Assert.Same(_user, found);

    _userRepository.Verify(x => x.LoadAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(null, key, value, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "FindAsync: it should return the user found by ID.")]
  public async Task Given_Id_When_FindAsync_Then_Found()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), userId: UserId.NewId(realmId));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    User found = await _manager.FindAsync($"  {user.EntityId}  ", PropertyName, includeId: true, _cancellationToken);
    Assert.Same(user, found);

    _userRepository.Verify(x => x.LoadAsync(user.Id, _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should return the user found by email address.")]
  public async Task Given_EmailAddress_When_FindAsync_Then_Found()
  {
    UserSettings userSettings = new(_userSettings.UniqueName, _userSettings.Password, requireUniqueEmail: true, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    Email email = new(_faker.Person.Email, isVerified: true);
    _user.SetEmail(email);
    _userRepository.Setup(x => x.LoadAsync(null, It.Is<IEmail>(e => e.Address == email.Address), _cancellationToken)).ReturnsAsync([_user]);

    User found = await _manager.FindAsync($"  {email}  ", PropertyName, includeId: false, _cancellationToken);
    Assert.Same(_user, found);

    _userRepository.Verify(x => x.LoadAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(null, It.Is<IEmail>(e => e.Address == email.Address), _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should return the user found by unique name.")]
  public async Task Given_UniqueName_When_FindAsync_Then_Found()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), userId: UserId.NewId(realmId));
    _userRepository.Setup(x => x.LoadAsync(realmId, user.UniqueName, _cancellationToken)).ReturnsAsync(user);

    User found = await _manager.FindAsync($"  {user.UniqueName}  ", PropertyName, includeId: false, _cancellationToken);
    Assert.Same(user, found);

    _userRepository.Verify(x => x.LoadAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(realmId, user.UniqueName, _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should throw UserNotFoundException when multiple users were found by email address.")]
  public async Task Given_MultipleEmail_When_FindAsync_Then_UserNotFoundException()
  {
    UserSettings userSettings = new(_userSettings.UniqueName, _userSettings.Password, requireUniqueEmail: true, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    Email email = new(_faker.Person.Email);
    User user = new(new UniqueName(userSettings.UniqueName, _faker.Internet.UserName()));
    _userRepository.Setup(x => x.LoadAsync(null, email, _cancellationToken)).ReturnsAsync([_user, user]);

    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _manager.FindAsync(email.Address, PropertyName, includeId: false, _cancellationToken));
    Assert.Null(exception.RealmId);
    Assert.Equal(email.Address, exception.User);
    Assert.Equal(PropertyName, exception.PropertyName);

    _userRepository.Verify(x => x.LoadAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(null, email, _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should throw UserNotFoundException when the input is too long.")]
  public async Task Given_TooLong_When_FindAsync_Then_UserNotFoundException()
  {
    UserSettings userSettings = new(_userSettings.UniqueName, _userSettings.Password, requireUniqueEmail: true, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    string user = $"{RandomStringGenerator.GetString(511)}:{RandomStringGenerator.GetString(511)}";
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _manager.FindAsync(user, PropertyName, includeId: true, _cancellationToken));
    Assert.Null(exception.RealmId);
    Assert.Equal(user, exception.User);
    Assert.Equal(PropertyName, exception.PropertyName);

    _userRepository.Verify(x => x.LoadAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should throw UserNotFoundException when the user was not found (EmailAddress).")]
  public async Task Given_EmailAddress_When_FindAsync_Then_UserNotFoundException()
  {
    UserSettings userSettings = new(_userSettings.UniqueName, _userSettings.Password, requireUniqueEmail: true, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    Email email = new(_faker.Person.Email);
    _userRepository.Setup(x => x.LoadAsync(null, email, _cancellationToken)).ReturnsAsync([]);

    string user = $"  {email}  ";
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _manager.FindAsync(user, PropertyName, includeId: false, _cancellationToken));
    Assert.Null(exception.RealmId);
    Assert.Equal(user, exception.User);
    Assert.Equal(PropertyName, exception.PropertyName);

    _userRepository.Verify(x => x.LoadAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(null, It.Is<UniqueName>(u => u.Value == email.Address), _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.LoadAsync(null, email, _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should throw UserNotFoundException when the user was not found (ID).")]
  public async Task Given_Id_When_FindAsync_Then_UserNotFoundException()
  {
    string user = $" {Guid.NewGuid()} ";
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _manager.FindAsync(user, PropertyName, includeId: true, _cancellationToken));
    Assert.Null(exception.RealmId);
    Assert.Equal(user, exception.User);
    Assert.Equal(PropertyName, exception.PropertyName);

    _userRepository.Verify(x => x.LoadAsync(It.Is<UserId>(id => id.RealmId == null && id.EntityId.ToString() == user.Trim()), _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should throw UserNotFoundException when the user was not found (UniqueName & CustomIdentifier).")]
  public async Task Given_UniqueNameAndCustomIdentifier_When_FindAsync_Then_UserNotFoundException()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    string user = "not:found";
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _manager.FindAsync(user, PropertyName, includeId: true, _cancellationToken));
    Assert.Equal(realmId.ToGuid(), exception.RealmId);
    Assert.Equal(user, exception.User);
    Assert.Equal(PropertyName, exception.PropertyName);

    _userRepository.Verify(x => x.LoadAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(realmId, It.Is<UniqueName>(u => u.Value == user), _cancellationToken), Times.Once());
    _userRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userRepository.Verify(x => x.LoadAsync(realmId, It.Is<Identifier>(k => k.Value == "not"), It.Is<CustomIdentifier>(v => v.Value == "found"), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when there is no change.")]
  public async Task Given_NoChange_When_SaveAsync_Then_Saved()
  {
    _user.ClearChanges();

    await _manager.SaveAsync(_user, _cancellationToken);

    _userRepository.Verify(x => x.SaveAsync(_user, _cancellationToken), Times.Once());
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Theory(DisplayName = "SaveAsync: it should save the user when there is no custom identifier conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoCustomIdentifierConflict_When_SaveAsync_Then_Saved(bool found)
  {
    Identifier key = new("Google");
    CustomIdentifier value = new(_faker.Random.String(length: 16, minChar: '0', maxChar: '9'));
    _user.ClearChanges();
    _user.SetCustomIdentifier(key, value);

    if (found)
    {
      _userQuerier.Setup(x => x.FindIdAsync(key, value, _cancellationToken)).ReturnsAsync(_user.Id);
    }

    await _manager.SaveAsync(_user, _cancellationToken);

    _userRepository.Verify(x => x.SaveAsync(_user, _cancellationToken), Times.Once());
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userQuerier.Verify(x => x.FindIdAsync(key, value, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when email addresses are not unique.")]
  public async Task Given_EmailNotUnique_When_SaveAsync_Then_Saved()
  {
    Email email = new(_faker.Person.Email);
    _user.ClearChanges();
    _user.SetEmail(email);

    await _manager.SaveAsync(_user, _cancellationToken);

    _userRepository.Verify(x => x.SaveAsync(_user, _cancellationToken), Times.Once());
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Theory(DisplayName = "SaveAsync: it should save the user when there is no email address conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoEmailAddressConflict_When_SaveAsync_Then_Saved(bool found)
  {
    UserSettings userSettings = new(_userSettings.UniqueName, _userSettings.Password, requireUniqueEmail: true, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    Email email = new(_faker.Person.Email);
    _user.ClearChanges();
    _user.SetEmail(email);
    _userQuerier.Setup(x => x.FindIdsAsync(email, _cancellationToken)).ReturnsAsync(found ? [_user.Id] : []);

    await _manager.SaveAsync(_user, _cancellationToken);

    _userRepository.Verify(x => x.SaveAsync(_user, _cancellationToken), Times.Once());
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never());
    _userQuerier.Verify(x => x.FindIdsAsync(email, _cancellationToken), Times.Once());
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Theory(DisplayName = "SaveAsync: it should save the user when there is no unique name conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved(bool found)
  {
    if (found)
    {
      _userQuerier.Setup(x => x.FindIdAsync(_user.UniqueName, _cancellationToken)).ReturnsAsync(_user.Id);
    }

    await _manager.SaveAsync(_user, _cancellationToken);

    _userRepository.Verify(x => x.SaveAsync(_user, _cancellationToken), Times.Once());
    _userQuerier.Verify(x => x.FindIdAsync(_user.UniqueName, _cancellationToken), Times.Once());
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never());
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "SaveAsync: it should throw CustomIdentifierAlreadyUsedException when there is a custom identifier conflict.")]
  public async Task Given_CustomIdentifierConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    Identifier key = new("Google");
    CustomIdentifier value = new(_faker.Random.String(length: 16, minChar: '0', maxChar: '9'));
    _userQuerier.Setup(x => x.FindIdAsync(key, value, _cancellationToken)).ReturnsAsync(_user.Id);

    User user = new(_user.UniqueName);
    user.SetCustomIdentifier(key, value);
    var exception = await Assert.ThrowsAsync<CustomIdentifierAlreadyUsedException>(async () => await _manager.SaveAsync(user, _cancellationToken));
    Assert.Equal(user.GetType().GetNamespaceQualifiedName(), exception.TypeName);
    Assert.Null(exception.RealmId);
    Assert.Equal(user.EntityId, exception.EntityId);
    Assert.Equal(_user.EntityId, exception.ConflictId);
    Assert.Equal(key.Value, exception.Key);
    Assert.Equal(value.Value, exception.Value);
  }

  [Fact(DisplayName = "SaveAsync: it should throw EmailAddressAlreadyUsedException when there is an email address conflict.")]
  public async Task Given_EmailConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    UserSettings userSettings = new(_userSettings.UniqueName, _userSettings.Password, requireUniqueEmail: true, requireConfirmedAccount: true);
    _applicationContext.SetupGet(x => x.UserSettings).Returns(userSettings);

    Email email = new(_faker.Person.Email);
    _userQuerier.Setup(x => x.FindIdsAsync(email, _cancellationToken)).ReturnsAsync([_user.Id]);

    User user = new(_user.UniqueName);
    user.SetEmail(email);
    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await _manager.SaveAsync(user, _cancellationToken));
    Assert.Null(exception.RealmId);
    Assert.Equal(user.EntityId, exception.UserId);
    Assert.Equal(_user.EntityId, exception.ConflictId);
    Assert.Equal(email.Address, exception.EmailAddress);
    Assert.Equal("Email", exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should throw UniqueNameAlreadyUsedException when there is a unique name conflict.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    _userQuerier.Setup(x => x.FindIdAsync(_user.UniqueName, _cancellationToken)).ReturnsAsync(_user.Id);

    User user = new(_user.UniqueName);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _manager.SaveAsync(user, _cancellationToken));
    Assert.Null(exception.RealmId);
    Assert.Equal(user.EntityId, exception.EntityId);
    Assert.Equal(_user.EntityId, exception.ConflictId);
    Assert.Equal(user.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
