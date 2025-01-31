﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Core.Roles.Validators;
using Logitar.Kraken.Core.Settings;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Commands;

public record CreateOrReplaceRoleResult(RoleModel? Role = null, bool Created = false);

public record CreateOrReplaceRoleCommand(Guid? Id, CreateOrReplaceRolePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceRoleResult>;

internal class CreateOrReplaceRoleCommandHandler : IRequestHandler<CreateOrReplaceRoleCommand, CreateOrReplaceRoleResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public CreateOrReplaceRoleCommandHandler(
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

  public async Task<CreateOrReplaceRoleResult> Handle(CreateOrReplaceRoleCommand command, CancellationToken cancellationToken)
  {
    IRoleSettings roleSettings = _applicationContext.RoleSettings;

    CreateOrReplaceRolePayload payload = command.Payload;
    new CreateOrReplaceRoleValidator(roleSettings.UniqueNameSettings).ValidateAndThrow(payload);

    RoleId? roleId = null;
    Role? role = null;
    if (command.Id.HasValue)
    {
      roleId = new(command.Id.Value);
      role = await _roleRepository.LoadAsync(roleId.Value, cancellationToken);
    }

    ActorId? actorId = _applicationContext.ActorId;
    UniqueName uniqueName = new(roleSettings.UniqueNameSettings, payload.UniqueName);

    bool created = false;
    if (role == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceRoleResult();
      }

      role = new(uniqueName, actorId, roleId);
      created = true;
    }

    Role reference = (command.Version.HasValue
      ? await _roleRepository.LoadAsync(role.Id, command.Version.Value, cancellationToken)
      : null) ?? role;

    if (reference.UniqueName != uniqueName)
    {
      role.SetUniqueName(uniqueName, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      role.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      role.Description = description;
    }

    role.SetCustomAttributes(payload.CustomAttributes, reference);

    role.Update(actorId);
    await _roleManager.SaveAsync(role, cancellationToken);

    RoleModel model = await _roleQuerier.ReadAsync(role, cancellationToken);
    return new CreateOrReplaceRoleResult(model, created);
  }
}
