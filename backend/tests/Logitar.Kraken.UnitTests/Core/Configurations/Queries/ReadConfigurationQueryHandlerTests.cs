using Logitar.Kraken.Contracts.Configurations;
using Moq;

namespace Logitar.Kraken.Core.Configurations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadConfigurationQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();

  private readonly ReadConfigurationQueryHandler _handler;

  public ReadConfigurationQueryHandlerTests()
  {
    _handler = new(_applicationContext.Object);
  }

  [Fact(DisplayName = "It should return the configuration from the cache.")]
  public async Task Given_Configuration_When_Handle_Then_ReturnedFromCache()
  {
    ConfigurationModel configuration = new();
    _applicationContext.Setup(x => x.Configuration).Returns(configuration);

    ReadConfigurationQuery query = new();
    ConfigurationModel model = await _handler.Handle(query, _cancellationToken);

    Assert.Same(configuration, model);
  }
}
