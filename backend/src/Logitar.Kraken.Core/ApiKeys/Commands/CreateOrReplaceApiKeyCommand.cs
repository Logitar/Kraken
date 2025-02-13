using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Core.ApiKeys.Validators;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Commands;

public record CreateOrReplaceApiKeyResult(ApiKeyModel? ApiKey = null, bool Created = false);

public record CreateOrReplaceApiKeyCommand(Guid? Id, CreateOrReplaceApiKeyPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceApiKeyResult>;

internal class CreateOrReplaceApiKeyCommandHandler : IRequestHandler<CreateOrReplaceApiKeyCommand, CreateOrReplaceApiKeyResult>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleManager _roleManager;

  public CreateOrReplaceApiKeyCommandHandler(
    IApiKeyQuerier apiKeyQuerier,
    IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    IRoleManager roleManager)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _roleManager = roleManager;
  }

  public async Task<CreateOrReplaceApiKeyResult> Handle(CreateOrReplaceApiKeyCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceApiKeyPayload payload = command.Payload;
    new CreateOrReplaceApiKeyValidator().ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    ApiKeyId apiKeyId = ApiKeyId.NewId(realmId);
    ApiKey? apiKey = null;
    if (command.Id.HasValue)
    {
      apiKeyId = new(command.Id.Value, realmId);
      apiKey = await _apiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    }

    ActorId? actorId = _applicationContext.ActorId;
    DisplayName name = new(payload.Name);

    string? secretString = null;
    bool created = false;
    if (apiKey == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceApiKeyResult();
      }

      Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out secretString);
      apiKey = new(secret, name, actorId, apiKeyId);
      created = true;
    }

    ApiKey reference = (command.Version.HasValue
      ? await _apiKeyRepository.LoadAsync(apiKey.Id, command.Version.Value, cancellationToken)
      : null) ?? apiKey;

    if (reference.Name != name)
    {
      apiKey.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      apiKey.Description = description;
    }
    if (reference.ExpiresOn != payload.ExpiresOn)
    {
      apiKey.ExpiresOn = payload.ExpiresOn;
    }

    apiKey.SetCustomAttributes(payload.CustomAttributes, reference);

    await SetRolesAsync(payload, reference, apiKey, actorId, cancellationToken);

    apiKey.Update(actorId);
    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    ApiKeyModel model = await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
    if (secretString != null)
    {
      model.XApiKey = new XApiKey(apiKey.Id, secretString).Encode();
    }
    return new CreateOrReplaceApiKeyResult(model, created);
  }

  private async Task SetRolesAsync(CreateOrReplaceApiKeyPayload payload, ApiKey reference, ApiKey apiKey, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyDictionary<RoleId, Role> roles = (await _roleManager.FindAsync(payload.Roles, cancellationToken))
      .ToDictionary(x => x.Value.Id, x => x.Value);

    foreach (RoleId roleId in reference.Roles)
    {
      if (!roles.ContainsKey(roleId))
      {
        apiKey.RemoveRole(roleId, actorId);
      }
    }

    foreach (Role role in roles.Values)
    {
      if (!reference.HasRole(role))
      {
        apiKey.AddRole(role, actorId);
      }
    }
  }
}
