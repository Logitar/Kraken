using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Core.ApiKeys.Validators;
using Logitar.Kraken.Core.Roles;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Commands;

public record UpdateApiKeyCommand(Guid Id, UpdateApiKeyPayload Payload) : Activity, IRequest<ApiKeyModel?>;

internal class UpdateApiKeyCommandHandler : IRequestHandler<UpdateApiKeyCommand, ApiKeyModel?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleManager _roleManager;

  public UpdateApiKeyCommandHandler(
    IApiKeyQuerier apiKeyQuerier,
    IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext,
    IRoleManager roleManager)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _roleManager = roleManager;
  }

  public async Task<ApiKeyModel?> Handle(UpdateApiKeyCommand command, CancellationToken cancellationToken)
  {
    UpdateApiKeyPayload payload = command.Payload;
    new UpdateApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyId apiKeyId = new(command.Id, _applicationContext.RealmId);
    ApiKey? apiKey = await _apiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    if (apiKey == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      apiKey.Name = new DisplayName(payload.Name);
    }
    if (payload.Description != null)
    {
      apiKey.Description = Description.TryCreate(payload.Description.Value);
    }
    if (payload.ExpiresOn.HasValue)
    {
      apiKey.ExpiresOn = payload.ExpiresOn.Value;
    }

    foreach (CustomAttributeModel customAttribute in payload.CustomAttributes)
    {
      apiKey.SetCustomAttribute(new Identifier(customAttribute.Key), customAttribute.Value);
    }

    IReadOnlyDictionary<string, Role> roles = await _roleManager.FindAsync(payload.Roles.Select(action => action.Role), cancellationToken);
    foreach (RoleAction action in payload.Roles)
    {
      Role role = roles[action.Role];
      switch (action.Action)
      {
        case CollectionAction.Add:
          apiKey.AddRole(role, actorId);
          break;
        case CollectionAction.Remove:
          apiKey.RemoveRole(role, actorId);
          break;
      }
    }

    apiKey.Update(actorId);
    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}
