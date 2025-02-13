using Logitar.Kraken.Contracts.Passwords;
using MediatR;

namespace Logitar.Kraken.Core.Passwords.Queries;

public record ReadOneTimePasswordQuery(Guid Id) : Activity, IRequest<OneTimePasswordModel?>;

internal class ReadOneTimePasswordQueryHandler : IRequestHandler<ReadOneTimePasswordQuery, OneTimePasswordModel?>
{
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;

  public ReadOneTimePasswordQueryHandler(IOneTimePasswordQuerier oneTimePasswordQuerier)
  {
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
  }

  public async Task<OneTimePasswordModel?> Handle(ReadOneTimePasswordQuery query, CancellationToken cancellationToken)
  {
    return await _oneTimePasswordQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
