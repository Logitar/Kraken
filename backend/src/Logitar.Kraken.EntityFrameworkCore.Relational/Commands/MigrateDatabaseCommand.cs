using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Commands;

public record MigrateDatabaseCommand : IRequest;

internal class MigrateDatabaseCommandHandler : IRequestHandler<MigrateDatabaseCommand>
{
  private readonly EventContext _eventContext;
  private readonly KrakenContext _krakenContext;

  public MigrateDatabaseCommandHandler(EventContext eventContext, KrakenContext krakenContext)
  {
    _eventContext = eventContext;
    _krakenContext = krakenContext;
  }

  public async Task Handle(MigrateDatabaseCommand _, CancellationToken cancellationToken)
  {
    await _eventContext.Database.MigrateAsync(cancellationToken);
    await _krakenContext.Database.MigrateAsync(cancellationToken);
  }
}
