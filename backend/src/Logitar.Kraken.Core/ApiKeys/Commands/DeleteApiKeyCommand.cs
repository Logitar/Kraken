using Logitar.Kraken.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Commands;

public record DeleteApiKeyCommand(Guid Id) : Activity, IRequest<ApiKeyModel?>;

internal class DeleteApiKeyCommandHandler : IRequestHandler<DeleteApiKeyCommand, ApiKeyModel?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;

  public DeleteApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext)
  {
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _apiKeyQuerier = apiKeyQuerier;
  }

  public async Task<ApiKeyModel?> Handle(DeleteApiKeyCommand command, CancellationToken cancellationToken)
  {
    ApiKeyId apiKeyId = new(command.Id, _applicationContext.RealmId);
    ApiKey? apiKey = await _apiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    if (apiKey == null)
    {
      return null;
    }
    ApiKeyModel result = await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);

    apiKey.Delete(_applicationContext.ActorId);
    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return result;
  }
}
