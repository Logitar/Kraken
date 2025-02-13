using Logitar.Kraken.Contracts.ApiKeys;
using Moq;

namespace Logitar.Kraken.Core.ApiKeys.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadApiKeyQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApiKeyQuerier> _apiKeyQuerier = new();

  private readonly ReadApiKeyQueryHandler _handler;

  public ReadApiKeyQueryHandlerTests()
  {
    _handler = new(_apiKeyQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when the API key could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    Assert.Null(await _handler.Handle(new ReadApiKeyQuery(Guid.NewGuid()), _cancellationToken));
  }

  [Fact(DisplayName = "It should return the API key found by ID.")]
  public async Task Given_Found_When_Handle_Then_ApiKeyReturned()
  {
    ApiKeyModel apiKey = new()
    {
      Id = Guid.NewGuid()
    };
    _apiKeyQuerier.Setup(x => x.ReadAsync(apiKey.Id, _cancellationToken)).ReturnsAsync(apiKey);

    ReadApiKeyQuery query = new(apiKey.Id);
    ApiKeyModel? result = await _handler.Handle(query, _cancellationToken);

    Assert.NotNull(result);
    Assert.Same(apiKey, result);
  }
}
