using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Localization.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class LanguageEvents : INotificationHandler<LanguageCreated>,
  INotificationHandler<LanguageDeleted>,
  INotificationHandler<LanguageLocaleChanged>,
  INotificationHandler<LanguageSetDefault>
{
  private readonly KrakenContext _context;
  private readonly ILogger<LanguageEvents> _logger;

  public LanguageEvents(KrakenContext context, ILogger<LanguageEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(LanguageCreated @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language == null)
    {
      LanguageId languageId = new(@event.StreamId);
      RealmEntity? realm = null;
      if (languageId.RealmId.HasValue)
      {
        Guid realmId = languageId.RealmId.Value.ToGuid();
        realm = await _context.Realms
          .SingleOrDefaultAsync(x => x.Id == realmId, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'Id={realmId}' could not be found.");
      }

      language = new(realm, @event);

      _context.Languages.Add(language);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
    else
    {
      _logger.LogWarning("{Event}: the language entity 'StreamId={Id}' already exists.", @event.GetType().Name, @event.StreamId);
    }
  }

  public async Task Handle(LanguageDeleted @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language == null)
    {
      _logger.LogWarning("{Event}: the language entity 'StreamId={StreamId}' is already deleted.", @event.GetType().Name, @event.StreamId);
    }
    else
    {
      _context.Languages.Remove(language);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(LanguageLocaleChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    LanguageEntity? language = await _context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language == null || language.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The language entity was expected to be at version {expectedVersion}, but was found at version {language?.Version ?? 0}.");
    }
    else if (language.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the language entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        language.Version,
        expectedVersion);
    }
    else
    {
      language.SetLocale(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(LanguageSetDefault @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    LanguageEntity? language = await _context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language == null || language.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The language entity was expected to be at version {expectedVersion}, but was found at version {language?.Version ?? 0}.");
    }
    else if (language.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the language entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        language.Version,
        expectedVersion);
    }
    else
    {
      language.SetDefault(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }
}
