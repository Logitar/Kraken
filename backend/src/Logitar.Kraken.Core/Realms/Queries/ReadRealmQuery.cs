using Logitar.Kraken.Contracts.Realms;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Queries;

/// <exception cref="TooManyResultsException"></exception>
public record ReadRealmQuery(Guid? Id, string? UniqueSlug) : IRequest<RealmModel?>;

internal class ReadRealmQueryHandler : IRequestHandler<ReadRealmQuery, RealmModel?>
{
  private readonly IRealmQuerier _realmQuerier;

  public ReadRealmQueryHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  public async Task<RealmModel?> Handle(ReadRealmQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, RealmModel> realms = new(capacity: 2);

    if (query.Id.HasValue)
    {
      RealmModel? realm = await _realmQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueSlug))
    {
      RealmModel? realm = await _realmQuerier.ReadAsync(query.UniqueSlug, cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw TooManyResultsException<RealmModel>.ExpectedSingle(realms.Count);
    }

    return realms.SingleOrDefault().Value;
  }
}
