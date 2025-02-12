using Logitar.Kraken.Contracts.Realms;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Commands;

public record DeleteRealmCommand(Guid Id) : IRequest<RealmModel?>;

internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, RealmModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public DeleteRealmCommandHandler(IApplicationContext applicationContext, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<RealmModel?> Handle(DeleteRealmCommand command, CancellationToken cancellationToken)
  {
    RealmId realmId = new(command.Id);
    Realm? realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    if (realm == null)
    {
      return null;
    }
    RealmModel result = await _realmQuerier.ReadAsync(realm, cancellationToken);

    realm.Delete(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return result;
  }
}
