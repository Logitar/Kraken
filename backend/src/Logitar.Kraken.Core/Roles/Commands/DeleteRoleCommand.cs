using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Commands;

public record DeleteRoleCommand(Guid Id) : Activity, IRequest<RoleModel?>;

internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, RoleModel?>
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;
  private readonly IUserRepository _userRepository;

  public DeleteRoleCommandHandler(
    IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext,
    IRoleQuerier roleQuerier,
    IRoleRepository roleRepository,
    IUserRepository userRepository)
  {
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
    _userRepository = userRepository;
  }

  public async Task<RoleModel?> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
  {
    RoleId roleId = new(command.Id, _applicationContext.RealmId);
    Role? role = await _roleRepository.LoadAsync(roleId, cancellationToken);
    if (role == null)
    {
      return null;
    }
    RoleModel result = await _roleQuerier.ReadAsync(role, cancellationToken);

    ActorId? actorId = _applicationContext.ActorId;

    IReadOnlyCollection<ApiKey> apiKeys = await _apiKeyRepository.LoadAsync(role.Id, cancellationToken);
    foreach (ApiKey apiKey in apiKeys)
    {
      apiKey.RemoveRole(role, actorId);
    }
    await _apiKeyRepository.SaveAsync(apiKeys, cancellationToken);

    IReadOnlyCollection<User> users = await _userRepository.LoadAsync(role.Id, cancellationToken);
    foreach (User user in users)
    {
      user.RemoveRole(role, actorId);
    }
    await _userRepository.SaveAsync(users, cancellationToken);

    role.Delete(actorId);
    await _roleRepository.SaveAsync(role, cancellationToken);

    return result;
  }
}
