using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Passwords;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Configurations;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly Dictionary<ActorId, ActorModel> _actors = [];
  private readonly ActorModel _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<ActorModel> actors)
  {
    foreach (ActorModel actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public ApiKeyModel ToApiKey(ApiKeyEntity source) => ToApiKey(source, GetRealm(source));
  public ApiKeyModel ToApiKey(ApiKeyEntity source, RealmModel? realm)
  {
    ApiKeyModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Description = source.Description,
      ExpiresOn = source.ExpiresOn?.AsUniversalTime(),
      AuthenticatedOn = source.AuthenticatedOn?.AsUniversalTime(),
      Realm = realm
    };

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttributeModel(customAttribute));
    }

    foreach (RoleEntity role in source.Roles)
    {
      destination.Roles.Add(ToRole(role, realm));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public ConfigurationModel ToConfiguration(Configuration source)
  {
    ConfigurationModel destination = new()
    {
      Secret = source.Secret.Value,
      UniqueNameSettings = new UniqueNameSettingsModel(source.UniqueNameSettings),
      PasswordSettings = new PasswordSettingsModel(source.PasswordSettings),
      LoggingSettings = new LoggingSettingsModel(source.LoggingSettings)
    };

    MapAggregate(source, destination);

    return destination;
  }

  public DictionaryModel ToDictionary(DictionaryEntity source) => ToDictionary(source, GetRealm(source));
  public DictionaryModel ToDictionary(DictionaryEntity source, RealmModel? realm)
  {
    if (source.Language == null)
    {
      throw new ArgumentException("The language is required.", nameof(source));
    }

    DictionaryModel destination = new()
    {
      Id = source.Id,
      Language = ToLanguage(source.Language),
      EntryCount = source.EntryCount,
      Realm = realm
    };

    foreach (KeyValuePair<string, string> entry in source.GetEntries())
    {
      destination.Entries.Add(new DictionaryEntryModel(entry));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public LanguageModel ToLanguage(LanguageEntity source) => ToLanguage(source, GetRealm(source));
  public LanguageModel ToLanguage(LanguageEntity source, RealmModel? realm)
  {
    LanguageModel destination = new()
    {
      Id = source.Id,
      IsDefault = source.IsDefault,
      Locale = new LocaleModel(source.Code),
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }

  public MessageModel ToMessage(MessageEntity source) => ToMessage(source, GetRealm(source));
  public MessageModel ToMessage(MessageEntity source, RealmModel? realm)
  {
    MessageModel destination = new()
    {
      Id = source.Id,
      Subject = source.Subject,
      Body = source.GetBody(),
      RecipientCount = source.RecipientCount,
      Sender = source.Sender == null ? ToSender(source) : ToSender(source.Sender, realm),
      Template = source.Template == null ? ToTemplate(source) : ToTemplate(source.Template, realm),
      IgnoreUserLocale = source.IgnoreUserLocale,
      Locale = source.Locale == null ? null : new LocaleModel(source.Locale),
      IsDemo = source.IsDemo,
      Status = source.Status,
      Realm = realm
    };

    foreach (RecipientEntity recipient in source.Recipients)
    {
      destination.Recipients.Add(ToRecipient(recipient, realm));
    }

    foreach (KeyValuePair<string, string> variable in source.GetVariables())
    {
      destination.Variables.Add(new Variable(variable));
    }

    foreach (KeyValuePair<string, string> resultData in source.GetResultData())
    {
      destination.ResultData.Add(new ResultData(resultData));
    }

    MapAggregate(source, destination);

    return destination;
  }
  public RecipientModel ToRecipient(RecipientEntity source, RealmModel? realm)
  {
    RecipientModel destination = new()
    {
      Type = source.Type,
      Address = source.Address,
      DisplayName = source.DisplayName,
      PhoneNumber = source.PhoneNumber,
    };

    if (source.User != null)
    {
      destination.User = ToUser(source.User, realm);
    }
    else if (source.UserId.HasValue)
    {
      throw new ArgumentException("The user is required.", nameof(source));
    }

    return destination;
  }

  public OneTimePasswordModel ToOneTimePassword(OneTimePasswordEntity source) => ToOneTimePassword(source, GetRealm(source));
  public OneTimePasswordModel ToOneTimePassword(OneTimePasswordEntity source, RealmModel? realm)
  {
    OneTimePasswordModel destination = new()
    {
      Id = source.Id,
      ExpiresOn = source.ExpiresOn?.AsUniversalTime(),
      MaximumAttempts = source.MaximumAttempts,
      AttemptCount = source.AttemptCount,
      HasValidationSucceeded = source.HasValidationSucceeded,
      Realm = realm
    };

    if (source.User != null)
    {
      destination.User = ToUser(source.User, realm);
    }
    else if (source.UserId.HasValue)
    {
      throw new ArgumentException("The user is required.", nameof(source));
    }

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttributeModel(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public RealmModel ToRealm(RealmEntity source)
  {
    RealmModel destination = new()
    {
      Id = source.Id,
      UniqueSlug = source.UniqueSlug,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Secret = source.Secret,
      Url = source.Url,
      UniqueNameSettings = source.GetUniqueNameSettings(),
      PasswordSettings = source.GetPasswordSettings(),
      RequireUniqueEmail = source.RequireUniqueEmail,
      RequireConfirmedAccount = source.RequireConfirmedAccount
    };

    MapAggregate(source, destination);

    return destination;
  }
  private RealmModel? GetRealm(ISegregatedEntity source)
  {
    RealmModel? destination = null;
    if (source.Realm != null)
    {
      destination = ToRealm(source.Realm);
    }
    else if (source.RealmId.HasValue)
    {
      throw new ArgumentException("The realm is required.", nameof(source));
    }
    return destination;
  }

  public RoleModel ToRole(RoleEntity source) => ToRole(source, GetRealm(source));
  public RoleModel ToRole(RoleEntity source, RealmModel? realm)
  {
    RoleModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Realm = realm
    };

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttributeModel(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public SenderModel ToSender(SenderEntity source) => ToSender(source, GetRealm(source));
  public SenderModel ToSender(SenderEntity source, RealmModel? realm)
  {
    SenderModel destination = new()
    {
      Id = source.Id,
      IsDefault = source.IsDefault,
      EmailAddress = source.EmailAddress,
      PhoneNumber = source.PhoneNumber,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Type = source.Type,
      Provider = source.Provider,
      Mailgun = source.GetMailgunSettings(),
      SendGrid = source.GetSendGridSettings(),
      Twilio = source.GetTwilioSettings(),
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }
  public SenderModel ToSender(MessageEntity source) => new()
  {
    IsDefault = source.SenderIsDefault,
    EmailAddress = source.SenderAddress,
    PhoneNumber = source.SenderPhoneNumber,
    DisplayName = source.SenderDisplayName,
    Type = source.SenderProvider.GetSenderType(),
    Provider = source.SenderProvider
  };

  public SessionModel ToSession(SessionEntity source) => ToSession(source, GetRealm(source));
  public SessionModel ToSession(SessionEntity source, RealmModel? realm)
  {
    if (source.User == null)
    {
      throw new ArgumentException("The user is required.", nameof(source));
    }

    SessionModel destination = new()
    {
      Id = source.Id,
      IsPersistent = source.IsPersistent,
      IsActive = source.IsActive,
      SignedOutBy = TryGetActor(source.SignedOutBy),
      SignedOutOn = source.SignedOutOn?.AsUniversalTime(),
      User = ToUser(source.User, realm)
    };

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttributeModel(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public TemplateModel ToTemplate(TemplateEntity source) => ToTemplate(source, GetRealm(source));
  public TemplateModel ToTemplate(TemplateEntity source, RealmModel? realm)
  {
    TemplateModel destination = new()
    {
      Id = source.Id,
      UniqueKey = source.UniqueKey,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Subject = source.Subject,
      Content = source.GetContent(),
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }
  public TemplateModel ToTemplate(MessageEntity source) => new()
  {
    UniqueKey = source.TemplateUniqueKey,
    DisplayName = source.TemplateDisplayName
  };

  public UserModel ToUser(UserEntity source) => ToUser(source, GetRealm(source));
  public UserModel ToUser(UserEntity source, RealmModel? realm)
  {
    UserModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      // TODO(fpion): complete
      Realm = realm
    };

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttributeModel(customAttribute));
    }

    foreach (UserIdentifierEntity identifier in source.Identifiers)
    {
      destination.CustomIdentifiers.Add(new CustomIdentifierModel(identifier.Key, identifier.Value));
    }

    foreach (RoleEntity role in source.Roles)
    {
      destination.Roles.Add(ToRole(role, realm));
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateRoot source, AggregateModel destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = TryGetActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = TryGetActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }
  private void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = TryGetActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = TryGetActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private ActorModel FindActor(string id) => FindActor(new ActorId(id));
  private ActorModel FindActor(ActorId id) => _actors[id];
  private ActorModel? TryGetActor(string? id) => string.IsNullOrWhiteSpace(id) ? null : TryGetActor(new ActorId(id));
  private ActorModel? TryGetActor(ActorId? id)
  {
    ActorModel? actor = null;
    if (id.HasValue)
    {
      _ = _actors.TryGetValue(id.Value, out actor);
    }
    return actor;
  }
}
