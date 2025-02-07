using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Localization.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class LanguageEntity : AggregateEntity
{
  public int LanguageId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public Guid Id { get; private set; }

  public bool IsDefault { get; private set; }

  public int LCID { get; private set; }
  public string Code { get; private set; } = string.Empty;
  public string CodeNormalized
  {
    get => Helper.Normalize(Code);
    private set { }
  }
  public string DisplayName { get; private set; } = string.Empty;
  public string EnglishName { get; private set; } = string.Empty;
  public string NativeName { get; private set; } = string.Empty;

  public DictionaryEntity? Dictionary { get; private set; }
  public List<ContentLocaleEntity> ContentLocales { get; private set; } = [];
  public List<FieldIndexEntity> FieldIndex { get; private set; } = [];
  public List<PublishedContentEntity> PublishedContents { get; private set; } = [];
  public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public LanguageEntity(RealmEntity? realm, LanguageCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;

    LanguageId language = new(@event.StreamId);
    Id = language.EntityId;

    IsDefault = @event.IsDefault;

    SetLocale(@event.Locale);
  }

  private LanguageEntity() : base()
  {
  }

  public void SetDefault(LanguageSetDefault @event)
  {
    Update(@event);

    IsDefault = @event.IsDefault;
  }

  public void SetLocale(LanguageLocaleChanged @event)
  {
    Update(@event);

    SetLocale(@event.Locale);
  }
  private void SetLocale(Locale locale)
  {
    LCID = locale.Culture.LCID;
    Code = locale.Culture.Name;
    DisplayName = locale.Culture.DisplayName;
    EnglishName = locale.Culture.EnglishName;
    NativeName = locale.Culture.NativeName;
  }

  public override string ToString() => $"{DisplayName} ({Code}) | {base.ToString()}";
}
