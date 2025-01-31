using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Core.Realms.Validators;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Commands;

public record CreateOrReplaceRealmResult(RealmModel? Realm = null, bool Created = false);

public record CreateOrReplaceRealmCommand(Guid? Id, CreateOrReplaceRealmPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceRealmResult>;

internal class CreateOrReplaceRealmCommandHandler : IRequestHandler<CreateOrReplaceRealmCommand, CreateOrReplaceRealmResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public CreateOrReplaceRealmCommandHandler(
    IApplicationContext applicationContext,
    IRealmManager realmManager,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<CreateOrReplaceRealmResult> Handle(CreateOrReplaceRealmCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealmPayload payload = command.Payload;
    new CreateOrReplaceRealmValidator().ValidateAndThrow(payload);

    RealmId? realmId = null;
    Realm? realm = null;
    if (command.Id.HasValue)
    {
      realmId = new(command.Id.Value);
      realm = await _realmRepository.LoadAsync(realmId.Value, cancellationToken);
    }

    ActorId? actorId = _applicationContext.GetActorId();
    Slug uniqueSlug = new(payload.UniqueSlug);
    JwtSecret secret = JwtSecret.CreateOrGenerate(payload.Secret);

    bool created = false;
    if (realm == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceRealmResult();
      }

      realm = new(uniqueSlug, secret, actorId, realmId);
      created = true;
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

    if (reference.Secret != secret)
    {
      realm.Secret = secret;
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

    realm.Update(actorId);
    await _realmManager.SaveAsync(realm, cancellationToken);

    RealmModel model = await _realmQuerier.ReadAsync(realm, cancellationToken);
    return new CreateOrReplaceRealmResult(model, created);
  }
}
