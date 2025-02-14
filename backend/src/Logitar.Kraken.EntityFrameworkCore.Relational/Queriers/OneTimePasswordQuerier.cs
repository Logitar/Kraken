using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Passwords;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class OneTimePasswordQuerier : IOneTimePasswordQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<OneTimePasswordEntity> _oneTimePasswords;

  public OneTimePasswordQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _oneTimePasswords = context.OneTimePasswords;
  }

  public async Task<OneTimePasswordModel> ReadAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken)
  {
    return await ReadAsync(oneTimePassword.Id, cancellationToken) ?? throw new InvalidOperationException($"The One-Time Password (OTP) entity 'StreamId={oneTimePassword.Id}' could not be found.");
  }
  public async Task<OneTimePasswordModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new OneTimePasswordId(id, _applicationContext.RealmId), cancellationToken);
  }
  public async Task<OneTimePasswordModel?> ReadAsync(OneTimePasswordId id, CancellationToken cancellationToken)
  {
    OneTimePasswordEntity? oneTimePassword = await _oneTimePasswords.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return oneTimePassword == null ? null : await MapAsync(oneTimePassword, cancellationToken);
  }

  private async Task<OneTimePasswordModel> MapAsync(OneTimePasswordEntity oneTimePassword, CancellationToken cancellationToken)
  {
    return (await MapAsync([oneTimePassword], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<OneTimePasswordModel>> MapAsync(IEnumerable<OneTimePasswordEntity> oneTimePasswords, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = oneTimePasswords.SelectMany(oneTimePassword => oneTimePassword.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return oneTimePasswords.Select(oneTimePassword => mapper.ToOneTimePassword(oneTimePassword, _applicationContext.Realm)).ToArray();
  }
}
