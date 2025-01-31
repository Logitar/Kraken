﻿using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Core.Realms;

public interface IRealmQuerier
{
  Task<RealmModel> ReadAsync(Realm realm, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<RealmModel>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
}
