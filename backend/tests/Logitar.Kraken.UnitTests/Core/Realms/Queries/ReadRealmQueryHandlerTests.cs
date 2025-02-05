using Logitar.Kraken.Contracts.Realms;
using Moq;

namespace Logitar.Kraken.Core.Realms.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadRealmQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRealmQuerier> _realmQuerier = new();

  private readonly ReadRealmQueryHandler _handler;

  private readonly RealmModel _oldWorld = new()
  {
    Id = Guid.NewGuid(),
    UniqueSlug = "the-old-world"
  };
  private readonly RealmModel _newWorld = new()
  {
    Id = Guid.NewGuid(),
    UniqueSlug = "the-new-world"
  };

  public ReadRealmQueryHandlerTests()
  {
    _handler = new(_realmQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when no realm was found.")]
  public async Task Given_NoRealmFound_When_Handle_Then_NullReturned()
  {
    ReadRealmQuery query = new(_oldWorld.Id, _newWorld.UniqueSlug);
    RealmModel? realm = await _handler.Handle(query, _cancellationToken);
    Assert.Null(realm);
  }

  [Fact(DisplayName = "It should return the realm found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_RealmReturned()
  {
    _realmQuerier.Setup(x => x.ReadAsync(_oldWorld.Id, _cancellationToken)).ReturnsAsync(_oldWorld);

    ReadRealmQuery query = new(_oldWorld.Id, UniqueSlug: null);
    RealmModel? realm = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(realm);
    Assert.Same(_oldWorld, realm);
  }

  [Fact(DisplayName = "It should return the realm found by unique name.")]
  public async Task Given_FoundByUniqueSlug_When_Handle_Then_RealmReturned()
  {
    _realmQuerier.Setup(x => x.ReadAsync(_newWorld.UniqueSlug, _cancellationToken)).ReturnsAsync(_newWorld);

    ReadRealmQuery query = new(Id: null, _newWorld.UniqueSlug);
    RealmModel? realm = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(realm);
    Assert.Same(_newWorld, realm);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many realms were found.")]
  public async Task Given_ManyRealmsFound_When_Handle_Then_TooManyResultsException()
  {
    _realmQuerier.Setup(x => x.ReadAsync(_oldWorld.Id, _cancellationToken)).ReturnsAsync(_oldWorld);
    _realmQuerier.Setup(x => x.ReadAsync(_newWorld.UniqueSlug, _cancellationToken)).ReturnsAsync(_newWorld);

    ReadRealmQuery query = new(_oldWorld.Id, _newWorld.UniqueSlug);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<RealmModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
