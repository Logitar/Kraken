using Bogus;
using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms.Events;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.Realms;

[Trait(Traits.Category, Categories.Unit)]
public class RealmTests
{
  private readonly Faker _faker = new();
  private readonly Realm _realm;

  public RealmTests()
  {
    Secret secret = new(RandomStringGenerator.GetString());
    _realm = new(new Slug("the-new-world"), secret);
  }

  [Theory(DisplayName = "ctor: it should construct the correct realm.")]
  [InlineData(null, true)]
  [InlineData("SYSTEM", false)]
  public void Given_Parameters_When_ctor_Then_CorrectRealmConstructed(string? actorIdValue, bool generateId)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);
    RealmId? id = generateId ? RealmId.NewId() : null;

    Realm realm = new(_realm.UniqueSlug, _realm.Secret, actorId, id);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, realm.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, realm.Id.ToGuid());
    }

    Assert.Equal(actorId, realm.CreatedBy);
    Assert.Equal(_realm.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(_realm.Secret, realm.Secret);
  }

  [Fact(DisplayName = "Delete: it should delete the realm.")]
  public void Given_Realm_When_Delete_Then_Deleted()
  {
    _realm.Delete();
    Assert.True(_realm.IsDeleted);

    _realm.ClearChanges();
    _realm.Delete();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);
  }

  [Fact(DisplayName = "Description: it should handle the updates correctly.")]
  public void Given_DescriptionUpdates_When_setDescription_Then_UpdatesHandledCorrectly()
  {
    _realm.ClearChanges();

    _realm.Description = null;
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.Description = new Description("This is the new world.");
    _realm.Update();
    Assert.True(_realm.HasChanges);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.Description?.Value == _realm.Description);
  }

  [Fact(DisplayName = "DisplayName: it should handle the updates correctly.")]
  public void Given_DisplayNameUpdates_When_setDisplayName_Then_UpdatesHandledCorrectly()
  {
    _realm.ClearChanges();

    _realm.DisplayName = _realm.DisplayName;
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.DisplayName = new DisplayName("The New World");
    _realm.Update();
    Assert.True(_realm.HasChanges);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.DisplayName?.Value == _realm.DisplayName);
  }

  [Fact(DisplayName = "PasswordSettings: it should handle the updates correctly.")]
  public void Given_PasswordSettingsUpdates_When_setPasswordSettings_Then_UpdatesHandledCorrectly()
  {
    _realm.ClearChanges();

    _realm.PasswordSettings = _realm.PasswordSettings;
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.PasswordSettings = new PasswordSettings(requiredLength: 6, requiredUniqueChars: 1, requireNonAlphanumeric: false, requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "PBKDF2");
    _realm.Update();
    Assert.True(_realm.HasChanges);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.PasswordSettings != null && updated.PasswordSettings.Equals(_realm.PasswordSettings));
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute.")]
  public void Given_CustomAttributes_When_RemoveCustomAttribute_Then_CustomAttributeRemoved()
  {
    Identifier key = new("IsPublic");
    _realm.SetCustomAttribute(key, bool.FalseString);
    _realm.Update();

    _realm.RemoveCustomAttribute(key);
    _realm.Update();
    Assert.False(_realm.CustomAttributes.ContainsKey(key));
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.CustomAttributes[key] == null);

    _realm.ClearChanges();
    _realm.RemoveCustomAttribute(key);
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);
  }

  [Fact(DisplayName = "RequireConfirmedAccount: it should handle the updates correctly.")]
  public void Given_RequireConfirmedAccountUpdates_When_setRequireConfirmedAccount_Then_UpdatesHandledCorrectly()
  {
    _realm.ClearChanges();

    _realm.RequireConfirmedAccount = _realm.RequireConfirmedAccount;
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.RequireConfirmedAccount = !_realm.RequireConfirmedAccount;
    _realm.Update();
    Assert.True(_realm.HasChanges);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.RequireConfirmedAccount == _realm.RequireConfirmedAccount);
  }

  [Fact(DisplayName = "RequireUniqueEmail: it should handle the updates correctly.")]
  public void Given_RequireUniqueEmailUpdates_When_setRequireUniqueEmail_Then_UpdatesHandledCorrectly()
  {
    _realm.ClearChanges();

    _realm.RequireUniqueEmail = _realm.RequireUniqueEmail;
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.RequireUniqueEmail = !_realm.RequireUniqueEmail;
    _realm.Update();
    Assert.True(_realm.HasChanges);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.RequireUniqueEmail == _realm.RequireUniqueEmail);
  }

  [Fact(DisplayName = "Secret: it should handle the updates correctly.")]
  public void Given_SecretUpdates_When_setSecret_Then_UpdatesHandledCorrectly()
  {
    _realm.ClearChanges();

    _realm.Secret = _realm.Secret;
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.Secret = new Secret(RandomStringGenerator.GetString());
    _realm.Update();
    Assert.True(_realm.HasChanges);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.Secret != null && updated.Secret.Equals(_realm.Secret));
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("IsPublic");
    _realm.SetCustomAttribute(key, bool.FalseString);
    _realm.Update();

    _realm.SetCustomAttribute(key, value!);
    _realm.Update();
    Assert.False(_realm.CustomAttributes.ContainsKey(key));
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.CustomAttributes[key] == null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("IsPublic");
    string value = $"  {bool.FalseString}  ";

    _realm.SetCustomAttribute(key, value);
    _realm.Update();
    Assert.Equal(_realm.CustomAttributes[key], value.Trim());
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _realm.ClearChanges();
    _realm.SetCustomAttribute(key, value.Trim());
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);
  }

  [Fact(DisplayName = "SetUniqueSlug: it should handle the updated correctly.")]
  public void Given_UniqueNameUpdates_When_setSetUniqueSlug_Then_UpdatesHandledCorrectly()
  {
    Slug uniqueSlug = new("the-old-world");
    _realm.SetUniqueSlug(uniqueSlug);
    Assert.Contains(_realm.Changes, change => change is RealmUniqueSlugChanged changed && changed.UniqueSlug == uniqueSlug);

    _realm.ClearChanges();
    _realm.SetUniqueSlug(uniqueSlug);
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(null)]
  [InlineData("The New World")]
  public void Given_Realm_When_ToString_Then_CorrectString(string? displayName)
  {
    if (displayName == null)
    {
      Assert.StartsWith(_realm.UniqueSlug.Value, _realm.ToString());
    }
    else
    {
      _realm.DisplayName = new(displayName);
      Assert.StartsWith(_realm.DisplayName.Value, _realm.ToString());
    }
  }

  [Fact(DisplayName = "UniqueNameSettings: it should handle the updates correctly.")]
  public void Given_UniqueNameSettingsUpdates_When_setUniqueNameSettings_Then_UpdatesHandledCorrectly()
  {
    _realm.ClearChanges();

    _realm.UniqueNameSettings = _realm.UniqueNameSettings;
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.UniqueNameSettings = new UniqueNameSettings(allowedCharacters: null);
    _realm.Update();
    Assert.True(_realm.HasChanges);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.UniqueNameSettings != null && updated.UniqueNameSettings.Equals(_realm.UniqueNameSettings));
  }

  [Theory(DisplayName = "Update: it should update the realm.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_RealmUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _realm.ClearChanges();
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.SetCustomAttribute(new Identifier("IsPublic"), bool.FalseString);
    _realm.Update(actorId);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }

  [Fact(DisplayName = "Url: it should handle the updates correctly.")]
  public void Given_UrlUpdates_When_setUrl_Then_UpdatesHandledCorrectly()
  {
    _realm.ClearChanges();

    _realm.Url = _realm.Url;
    _realm.Update();
    Assert.False(_realm.HasChanges);
    Assert.Empty(_realm.Changes);

    _realm.Url = new Url($"https://www.{_faker.Internet.DomainName()}");
    _realm.Update();
    Assert.True(_realm.HasChanges);
    Assert.Contains(_realm.Changes, change => change is RealmUpdated updated && updated.Url != null && updated.Url.Value == _realm.Url);
  }
}
