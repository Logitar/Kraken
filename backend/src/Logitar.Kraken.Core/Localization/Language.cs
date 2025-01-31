﻿using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization.Events;

namespace Logitar.Kraken.Core.Localization;

public class Language : AggregateRoot
{
  public new LanguageId Id => new(base.Id);

  public bool IsDefault { get; private set; }

  private Locale? _locale = null;
  public Locale Locale => _locale ?? throw new InvalidOperationException("The language has not been initialized.");

  public Language(Locale locale, bool isDefault = false, ActorId? actorId = null, LanguageId? languageId = null)
    : base(languageId?.StreamId)
  {
    Raise(new LanguageCreated(isDefault, locale), actorId);
  }
  protected virtual void Handle(LanguageCreated @event)
  {
    IsDefault = @event.IsDefault;

    _locale = @event.Locale;
  }

  public override string ToString() => $"{_locale} | {base.ToString()}";
}
