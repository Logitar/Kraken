using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Roles.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Commands;

public record UpdateRoleCommand(Guid Id, UpdateRolePayload Payload) : Activity, IRequest<RoleModel?>;

internal class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public UpdateRoleCommandHandler(
    IApplicationContext applicationContext,
    IRoleManager roleManager,
    IRoleQuerier roleQuerier,
    IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<RoleModel?> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateRolePayload payload = command.Payload;
    new UpdateRoleValidator(uniqueNameSettings).ValidateAndThrow(payload);

    RoleId roleId = new(command.Id, _applicationContext.RealmId);
    Role? role = await _roleRepository.LoadAsync(roleId, cancellationToken);
    if (role == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      role.SetUniqueName(new UniqueName(uniqueNameSettings, payload.UniqueName));
    }
    if (payload.DisplayName != null)
    {
      role.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      role.Description = Description.TryCreate(payload.Description.Value);
    }

    foreach (CustomAttributeModel customAttribute in payload.CustomAttributes)
    {
      role.SetCustomAttribute(new Identifier(customAttribute.Key), customAttribute.Value);
    }

    role.Update(actorId);
    await _roleManager.SaveAsync(role, cancellationToken);

    return await _roleQuerier.ReadAsync(role, cancellationToken);
  }
}
