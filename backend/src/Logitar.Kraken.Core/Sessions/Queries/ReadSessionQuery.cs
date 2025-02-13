using Logitar.Kraken.Contracts.Sessions;
using MediatR;

namespace Logitar.Kraken.Core.Sessions.Queries;

public record ReadSessionQuery(Guid Id) : Activity, IRequest<SessionModel?>;

internal class ReadSessionQueryHandler : IRequestHandler<ReadSessionQuery, SessionModel?>
{
  private readonly ISessionQuerier _sessionQuerier;

  public ReadSessionQueryHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<SessionModel?> Handle(ReadSessionQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
