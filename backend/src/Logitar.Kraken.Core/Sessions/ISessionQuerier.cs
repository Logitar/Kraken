using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Sessions;

namespace Logitar.Kraken.Core.Sessions;

public interface ISessionQuerier
{
  Task<SessionModel> ReadAsync(Session session, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<SessionModel>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken = default);
}
