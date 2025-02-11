using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Core.Realms.Validators;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Commands;

public record UpdateRealmCommand(Guid Id, UpdateRealmPayload Payload) : IRequest<RealmModel?>;

internal class UpdateRealmCommandHandler : IRequestHandler<UpdateRealmCommand, RealmModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public UpdateRealmCommandHandler(
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

  public async Task<RealmModel?> Handle(UpdateRealmCommand command, CancellationToken cancellationToken)
  {
    UpdateRealmPayload payload = command.Payload;
    new UpdateRealmValidator().ValidateAndThrow(payload);

    RealmId realmId = new(command.Id);
    Realm? realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueSlug))
    {
      Slug uniqueSlug = new(payload.UniqueSlug);
      realm.SetUniqueSlug(uniqueSlug, actorId);
    }
    if (payload.DisplayName != null)
    {
      realm.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      realm.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Secret != null)
    {
      realm.Secret = JwtSecret.CreateOrGenerate(payload.Secret);
    }
    if (payload.Url != null)
    {
      realm.Url = Url.TryCreate(payload.Url.Value);
    }

    if (payload.UniqueNameSettings != null)
    {
      realm.UniqueNameSettings = new UniqueNameSettings(payload.UniqueSlug);
    }
    if (payload.PasswordSettings != null)
    {
      realm.PasswordSettings = new PasswordSettings(payload.PasswordSettings);
    }
    if (payload.RequireUniqueEmail.HasValue)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail.Value;
    }
    if (payload.RequireConfirmedAccount.HasValue)
    {
      realm.RequireConfirmedAccount = payload.RequireConfirmedAccount.Value;
    }

    foreach (CustomAttributeModel customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      realm.SetCustomAttribute(key, customAttribute.Value);
    }

    realm.Update(actorId);
    await _realmManager.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
