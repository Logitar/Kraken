using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Realms.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class RealmEntity : AggregateEntity
{
  public int RealmId { get; private set; }

  public Guid Id { get; private set; }

  public string UniqueSlug { get; private set; } = string.Empty;
  public string UniqueSlugNormalized
  {
    get => Helper.Normalize(UniqueSlug);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string Secret { get; private set; } = string.Empty;
  public string? Url { get; private set; }

  public string? AllowedUniqueNameCharacters { get; private set; }
  public int RequiredPasswordLength { get; private set; }
  public int RequiredPasswordUniqueChars { get; private set; }
  public bool PasswordsRequireNonAlphanumeric { get; private set; }
  public bool PasswordsRequireLowercase { get; private set; }
  public bool PasswordsRequireUppercase { get; private set; }
  public bool PasswordsRequireDigit { get; private set; }
  public string PasswordHashingStrategy { get; private set; } = string.Empty;
  public bool RequireUniqueEmail { get; private set; }
  public bool RequireConfirmedAccount { get; private set; }

  public List<ApiKeyEntity> ApiKeys { get; private set; } = [];
  public List<OneTimePasswordEntity> OneTimePasswords { get; private set; } = [];
  public List<RoleEntity> Roles { get; private set; } = [];
  public List<SessionEntity> Sessions { get; private set; } = [];
  public List<UserEntity> Users { get; private set; } = [];
  public List<UserIdentifierEntity> UserIdentifiers { get; private set; } = [];

  public RealmEntity(RealmCreated @event) : base(@event)
  {
    RealmId realmId = new(@event.StreamId);
    Id = realmId.ToGuid();

    UniqueSlug = @event.UniqueSlug.Value;

    Secret = @event.Secret.Value;

    SetUniqueNameSettings(@event.UniqueNameSettings);
    SetPasswordSettings(@event.PasswordSettings);
    RequireUniqueEmail = @event.RequireUniqueEmail;
    RequireConfirmedAccount = @event.RequireConfirmedAccount;
  }

  private RealmEntity() : base()
  {
  }

  public void SetUniqueSlug(RealmUniqueSlugChanged @event)
  {
    Update(@event);

    UniqueSlug = @event.UniqueSlug.Value;
  }

  public void Update(RealmUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Secret != null)
    {
      Secret = @event.Secret.Value;
    }
    if (@event.Url != null)
    {
      Url = @event.Url.Value?.Value;
    }

    if (@event.UniqueNameSettings != null)
    {
      SetUniqueNameSettings(@event.UniqueNameSettings);
    }
    if (@event.PasswordSettings != null)
    {
      SetPasswordSettings(@event.PasswordSettings);
    }
    if (@event.RequireUniqueEmail.HasValue)
    {
      RequireUniqueEmail = @event.RequireUniqueEmail.Value;
    }
    if (@event.RequireConfirmedAccount.HasValue)
    {
      RequireConfirmedAccount = @event.RequireConfirmedAccount.Value;
    }
  }

  private void SetUniqueNameSettings(IUniqueNameSettings uniqueName)
  {
    AllowedUniqueNameCharacters = uniqueName.AllowedCharacters;
  }
  private void SetPasswordSettings(IPasswordSettings password)
  {
    RequiredPasswordLength = password.RequiredLength;
    RequiredPasswordUniqueChars = password.RequiredUniqueChars;
    PasswordsRequireNonAlphanumeric = password.RequireNonAlphanumeric;
    PasswordsRequireLowercase = password.RequireLowercase;
    PasswordsRequireUppercase = password.RequireUppercase;
    PasswordsRequireDigit = password.RequireDigit;
    PasswordHashingStrategy = password.HashingStrategy;
  }

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
