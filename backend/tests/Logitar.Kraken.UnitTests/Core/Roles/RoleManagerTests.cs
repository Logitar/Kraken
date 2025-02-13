using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;
using Moq;

namespace Logitar.Kraken.Core.Roles;

[Trait(Traits.Category, Categories.Unit)]
public class RoleManagerTests
{
  private const string PropertyName = "Roles";

  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IRoleQuerier> _roleQuerier = new();
  private readonly Mock<IRoleRepository> _roleRepository = new();

  private readonly RoleManager _manager;

  public RoleManagerTests()
  {
    _manager = new(_applicationContext.Object, _roleQuerier.Object, _roleRepository.Object);
  }

  [Fact(DisplayName = "FindAsync: it should not load roles when there is no ID.")]
  public async Task Given_NoId_When_FindAsync_Then_NotLoaded()
  {
    IReadOnlyDictionary<string, Role> results = await _manager.FindAsync(values: [], PropertyName, _cancellationToken);
    Assert.Empty(results);

    _roleRepository.Verify(x => x.LoadAsync(It.IsAny<IEnumerable<RoleId>>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should not query roles when there is no unique name.")]
  public async Task Given_NoUniqueName_When_FindAsync_Then_NotQueried()
  {
    UniqueNameSettings uniqueNameSettings = new();
    Role admin = new(new UniqueName(uniqueNameSettings, "admin"));

    _roleRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<RoleId>>(y => y.SequenceEqual(new RoleId[] { admin.Id })),
      _cancellationToken)).ReturnsAsync([admin]);

    string[] roles = [admin.EntityId.ToString()];
    IReadOnlyDictionary<string, Role> results = await _manager.FindAsync(roles, PropertyName, _cancellationToken);

    Assert.Single(results);
    Assert.Contains(results, r => r.Key == roles.Single() && r.Value.Equals(admin));

    _roleQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should return the roles found.")]
  public async Task Given_RolesFound_When_FindAsync_Then_RolesReturned()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    UniqueNameSettings uniqueNameSettings = new();
    Role admin = new(new UniqueName(uniqueNameSettings, "admin"), roleId: RoleId.NewId(realmId));
    Role buyer = new(new UniqueName(uniqueNameSettings, "buyer"), roleId: RoleId.NewId(realmId));
    Role guest = new(new UniqueName(uniqueNameSettings, "guest"), roleId: RoleId.NewId(realmId));
    Role seller = new(new UniqueName(uniqueNameSettings, "seller"), roleId: RoleId.NewId(realmId));

    _roleQuerier.Setup(x => x.FindIdsAsync(
      It.Is<IEnumerable<string>>(y => y.SequenceEqual(new string[] { $"  {admin.UniqueName}  ", seller.UniqueName.Value })),
      _cancellationToken)).ReturnsAsync([admin.Id, seller.Id]);

    _roleRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<RoleId>>(y => y.SequenceEqual(new RoleId[] { buyer.Id, guest.Id, admin.Id, seller.Id })),
      _cancellationToken)).ReturnsAsync([admin, buyer, guest, seller]);

    string[] roles = [$"  {admin.UniqueName}  ", buyer.EntityId.ToString(), guest.EntityId.ToString(), seller.UniqueName.Value];
    IReadOnlyDictionary<string, Role> results = await _manager.FindAsync(roles, PropertyName, _cancellationToken);

    Assert.Equal(4, results.Count);
    Assert.Contains(results, r => r.Key == $"  {admin.UniqueName}  " && r.Value.Equals(admin));
    Assert.Contains(results, r => r.Key == buyer.EntityId.ToString() && r.Value.Equals(buyer));
    Assert.Contains(results, r => r.Key == guest.EntityId.ToString() && r.Value.Equals(guest));
    Assert.Contains(results, r => r.Key == seller.UniqueName.Value && r.Value.Equals(seller));
  }

  [Fact(DisplayName = "FindAsync: it should throw RolesNotFoundException when there is missing roles.")]
  public async Task Given_MissingRoles_When_FindAsync_Then_RolesNotFoundException()
  {
    UniqueNameSettings uniqueNameSettings = new();
    Role admin = new(new UniqueName(uniqueNameSettings, "admin"));
    Role buyer = new(new UniqueName(uniqueNameSettings, "buyer"));
    Role guest = new(new UniqueName(uniqueNameSettings, "guest"));
    Role seller = new(new UniqueName(uniqueNameSettings, "seller"));

    _roleQuerier.Setup(x => x.FindIdsAsync(
      It.Is<IEnumerable<string>>(y => y.SequenceEqual(new string[] { admin.UniqueName.Value, seller.UniqueName.Value })),
      _cancellationToken)).ReturnsAsync([admin.Id]);

    _roleRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<RoleId>>(y => y.SequenceEqual(new RoleId[] { buyer.Id, guest.Id, admin.Id })),
      _cancellationToken)).ReturnsAsync([admin, guest]);

    string[] roles = [admin.UniqueName.Value, buyer.EntityId.ToString(), guest.EntityId.ToString(), seller.UniqueName.Value];
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await _manager.FindAsync(roles, PropertyName, _cancellationToken));
    Assert.Equal([buyer.EntityId.ToString(), seller.UniqueName.Value], exception.Roles);
    Assert.Equal(PropertyName, exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should save a role.")]
  public async Task Given_NoChange_When_SaveAsync_Then_Saved()
  {
    Role role = new(new UniqueName(new UniqueNameSettings(), "admin"));
    role.ClearChanges();

    await _manager.SaveAsync(role, _cancellationToken);

    _roleQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _roleRepository.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "SaveAsync: it should save the role when there is no unique name conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved(bool found)
  {
    Role role = new(new UniqueName(new UniqueNameSettings(), "admin"));
    if (found)
    {
      _roleQuerier.Setup(x => x.FindIdAsync(role.UniqueName, _cancellationToken)).ReturnsAsync(role.Id);
    }

    await _manager.SaveAsync(role, _cancellationToken);

    _roleQuerier.Verify(x => x.FindIdAsync(role.UniqueName, _cancellationToken), Times.Once);
    _roleRepository.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should throw UniqueKeyAlreadyUsedException when the unique name is already used.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueKeyAlreadyUsedException()
  {
    Role role = new(new UniqueName(new UniqueNameSettings(), "admin"));

    RoleId conflictId = RoleId.NewId(realmId: null);
    _roleQuerier.Setup(x => x.FindIdAsync(role.UniqueName, _cancellationToken)).ReturnsAsync(conflictId);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _manager.SaveAsync(role, _cancellationToken));
    Assert.Equal(role.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(conflictId.EntityId, exception.ConflictId);
    Assert.Equal(role.EntityId, exception.EntityId);
    Assert.Equal(role.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
