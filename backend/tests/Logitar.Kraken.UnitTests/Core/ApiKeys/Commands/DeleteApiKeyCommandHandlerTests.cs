using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteApiKeyCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApiKeyQuerier> _apiKeyQuerier = new();
  private readonly Mock<IApiKeyRepository> _apiKeyRepository = new();
  private readonly Mock<IApplicationContext> _applicationContext = new();

  private readonly DeleteApiKeyCommandHandler _handler;

  private readonly ApiKey _apiKey;

  public DeleteApiKeyCommandHandlerTests()
  {
    _handler = new(_apiKeyQuerier.Object, _apiKeyRepository.Object, _applicationContext.Object);

    Base64Password secret = new(Guid.NewGuid().ToString());
    DisplayName name = new("Test");
    _apiKey = new(secret, name, actorId: null, ApiKeyId.NewId(_realmId));
    _apiKeyRepository.Setup(x => x.LoadAsync(_apiKey.Id, _cancellationToken)).ReturnsAsync(_apiKey);
  }

  [Fact(DisplayName = "It should delete an existing API key.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    _apiKeyRepository.Setup(x => x.LoadAsync(_realmId, _cancellationToken)).ReturnsAsync([_apiKey]);

    ApiKeyModel model = new();
    _apiKeyQuerier.Setup(x => x.ReadAsync(_apiKey, _cancellationToken)).ReturnsAsync(model);

    DeleteApiKeyCommand command = new(_apiKey.EntityId);
    ApiKeyModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(_apiKey.IsDeleted);

    _apiKeyRepository.Verify(x => x.SaveAsync(_apiKey, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the API key could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteApiKeyCommand command = new(_apiKey.EntityId);
    ApiKeyModel? apiKey = await _handler.Handle(command, _cancellationToken);
    Assert.Null(apiKey);
  }
}
