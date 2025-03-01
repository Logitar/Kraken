﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Core.Realms.Validators;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Commands;

public record CreateOrReplaceRealmResult(RealmModel? Realm = null, bool Created = false);

/// <exception cref="UniqueSlugAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceRealmCommand(Guid? Id, CreateOrReplaceRealmPayload Payload, long? Version) : IRequest<CreateOrReplaceRealmResult>;

internal class CreateOrReplaceRealmCommandHandler : IRequestHandler<CreateOrReplaceRealmCommand, CreateOrReplaceRealmResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;
  private readonly ISecretHelper _secretHelper;

  public CreateOrReplaceRealmCommandHandler(
    IApplicationContext applicationContext,
    IRealmManager realmManager,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository,
    ISecretHelper secretHelper)
  {
    _applicationContext = applicationContext;
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
    _secretHelper = secretHelper;
  }

  public async Task<CreateOrReplaceRealmResult> Handle(CreateOrReplaceRealmCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealmPayload payload = command.Payload;
    new CreateOrReplaceRealmValidator().ValidateAndThrow(payload);

    RealmId realmId = RealmId.NewId();
    Realm? realm = null;
    if (command.Id.HasValue)
    {
      realmId = new(command.Id.Value);
      realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    }

    Slug uniqueSlug = new(payload.UniqueSlug);
    ActorId? actorId = _applicationContext.ActorId;

    bool created = false;
    if (realm == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceRealmResult();
      }

      Secret secret = string.IsNullOrWhiteSpace(payload.Secret) ? _secretHelper.Generate(realmId) : _secretHelper.Encrypt(payload.Secret, realmId);
      realm = new(uniqueSlug, secret, actorId, realmId);
      created = true;
    }
    else if (payload.Secret != null)
    {
      realm.Secret = string.IsNullOrWhiteSpace(payload.Secret) ? _secretHelper.Generate(realm.Id) : _secretHelper.Encrypt(payload.Secret, realm.Id);
    }

    Realm reference = (command.Version.HasValue
      ? await _realmRepository.LoadAsync(realm.Id, command.Version.Value, cancellationToken)
      : null) ?? realm;

    if (reference.UniqueSlug != uniqueSlug)
    {
      realm.SetUniqueSlug(uniqueSlug, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      realm.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      realm.Description = description;
    }

    Url? url = Url.TryCreate(payload.Url);
    if (reference.Url != url)
    {
      realm.Url = url;
    }

    UniqueNameSettings uniqueNameSettings = new(payload.UniqueNameSettings);
    if (reference.UniqueNameSettings != uniqueNameSettings)
    {
      realm.UniqueNameSettings = uniqueNameSettings;
    }
    PasswordSettings passwordSettings = new(payload.PasswordSettings);
    if (reference.PasswordSettings != passwordSettings)
    {
      realm.PasswordSettings = passwordSettings;
    }
    if (reference.RequireUniqueEmail != payload.RequireUniqueEmail)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail;
    }
    if (reference.RequireConfirmedAccount != payload.RequireConfirmedAccount)
    {
      realm.RequireConfirmedAccount = payload.RequireConfirmedAccount;
    }

    realm.SetCustomAttributes(payload.CustomAttributes, reference);

    realm.Update(actorId);
    await _realmManager.SaveAsync(realm, cancellationToken);

    RealmModel model = await _realmQuerier.ReadAsync(realm, cancellationToken);
    return new CreateOrReplaceRealmResult(model, created);
  }
}
