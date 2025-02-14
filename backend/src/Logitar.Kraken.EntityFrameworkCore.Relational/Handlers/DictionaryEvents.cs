using Logitar.EventSourcing;
using Logitar.Kraken.Core.Dictionaries.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class DictionaryEvents : INotificationHandler<DictionaryCreated>,
  INotificationHandler<DictionaryDeleted>,
  INotificationHandler<DictionaryLanguageChanged>,
  INotificationHandler<DictionaryUpdated>
{
  private readonly KrakenContext _context;
  private readonly ILogger<DictionaryEvents> _logger;

  public DictionaryEvents(KrakenContext context, ILogger<DictionaryEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(DictionaryCreated @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await FindAsync(@event, cancellationToken);
    if (dictionary == null)
    {
      LanguageEntity language = await _context.FindLanguageAsync(@event.LanguageId, cancellationToken);

      dictionary = new(language, @event);

      _context.Dictionaries.Add(dictionary);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
    else
    {
      _logger.LogWarning("{Event}: the dictionary entity 'StreamId={Id}' already exists.", @event.GetType().Name, @event.StreamId);
    }
  }

  public async Task Handle(DictionaryDeleted @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await FindAsync(@event, cancellationToken);
    if (dictionary == null)
    {
      _logger.LogWarning("{Event}: the dictionary entity 'StreamId={StreamId}' is already deleted.", @event.GetType().Name, @event.StreamId);
    }
    else
    {
      _context.Dictionaries.Remove(dictionary);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(DictionaryLanguageChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    DictionaryEntity? dictionary = await FindAsync(@event, cancellationToken);
    if (dictionary == null || dictionary.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The dictionary entity was expected to be at version {expectedVersion}, but was found at version {dictionary?.Version ?? 0}.");
    }
    else if (dictionary.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the dictionary entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        dictionary.Version,
        expectedVersion);
    }
    else
    {
      LanguageEntity language = await _context.FindLanguageAsync(@event.LanguageId, cancellationToken);

      dictionary.SetLanguage(language, @event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(DictionaryUpdated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    DictionaryEntity? dictionary = await FindAsync(@event, cancellationToken);
    if (dictionary == null || dictionary.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The dictionary entity was expected to be at version {expectedVersion}, but was found at version {dictionary?.Version ?? 0}.");
    }
    else if (dictionary.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the dictionary entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        dictionary.Version,
        expectedVersion);
    }
    else
    {
      dictionary.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  private async Task<DictionaryEntity?> FindAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    return await _context.Dictionaries.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
  }
}
