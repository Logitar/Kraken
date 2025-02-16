using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Caching;
using Moq;

namespace Logitar.Kraken.Core.Configurations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadConfigurationQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICacheService> _cacheService = new();

  private readonly ReadConfigurationQueryHandler _handler;

  public ReadConfigurationQueryHandlerTests()
  {
    _handler = new(_cacheService.Object);
  }

  [Fact(DisplayName = "It should return the configuration found in the cache.")]
  public async Task Given_Cached_When_Handle_Then_Returned()
  {
    ConfigurationModel configuration = new();
    _cacheService.SetupGet(x => x.Configuration).Returns(configuration);

    ReadConfigurationQuery query = new();
    ConfigurationModel result = await _handler.Handle(query, _cancellationToken);
    Assert.Same(configuration, result);
  }

  [Fact(DisplayName = "It should throw InvalidOperationException when the configuration was not cached.")]
  public async Task Given_NotCached_When_Handle_Then_InvalidOperationException()
  {
    ReadConfigurationQuery query = new();
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal("The configuration was not found in the cache.", exception.Message);
  }
}
