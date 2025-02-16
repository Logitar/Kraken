using Logitar.Kraken.Core.Tokens;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Realms;

[Trait(Traits.Category, Categories.Unit)]
public class RealmManagerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRealmQuerier> _realmQuerier = new();
  private readonly Mock<IRealmRepository> _realmRepository = new();

  private readonly RealmManager _manager;

  public RealmManagerTests()
  {
    _manager = new(_realmQuerier.Object, _realmRepository.Object);
  }

  [Fact(DisplayName = "It should save a realm.")]
  public async Task Given_NoChange_When_SaveAsync_Then_Saved()
  {
    Secret secret = new(RandomStringGenerator.GetString());
    Realm realm = new(new Slug("test"), secret);
    realm.ClearChanges();

    await _manager.SaveAsync(realm, _cancellationToken);

    _realmQuerier.Verify(x => x.FindIdAsync(It.IsAny<Slug>(), It.IsAny<CancellationToken>()), Times.Never);
    _realmRepository.Verify(x => x.SaveAsync(realm, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should save the realm when there is no unique name conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved(bool found)
  {
    Secret secret = new(RandomStringGenerator.GetString());
    Realm realm = new(new Slug("test"), secret);
    if (found)
    {
      _realmQuerier.Setup(x => x.FindIdAsync(realm.UniqueSlug, _cancellationToken)).ReturnsAsync(realm.Id);
    }

    await _manager.SaveAsync(realm, _cancellationToken);

    _realmQuerier.Verify(x => x.FindIdAsync(realm.UniqueSlug, _cancellationToken), Times.Once);
    _realmRepository.Verify(x => x.SaveAsync(realm, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw UniqueKeyAlreadyUsedException when the unique name is already used.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueKeyAlreadyUsedException()
  {
    Secret secret = new(RandomStringGenerator.GetString());
    Realm realm = new(new Slug("test"), secret);

    RealmId conflictId = RealmId.NewId();
    _realmQuerier.Setup(x => x.FindIdAsync(realm.UniqueSlug, _cancellationToken)).ReturnsAsync(conflictId);

    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await _manager.SaveAsync(realm, _cancellationToken));
    Assert.Equal(conflictId.ToGuid(), exception.ConflictId);
    Assert.Equal(realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal(realm.UniqueSlug.Value, exception.UniqueSlug);
    Assert.Equal("UniqueSlug", exception.PropertyName);
  }
}
