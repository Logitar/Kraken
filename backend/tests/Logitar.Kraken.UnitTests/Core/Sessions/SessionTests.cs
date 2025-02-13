using Bogus;
using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Sessions.Events;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Users;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.Sessions;

[Trait(Traits.Category, Categories.Unit)]
public class SessionTests
{
  private readonly Faker _faker = new();

  private readonly User _user;
  private readonly Session _session;

  public SessionTests()
  {
    _user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    _session = new(_user);
  }

  [Theory(DisplayName = "ctor: it should construct the correct session.")]
  [InlineData(null, true)]
  [InlineData("SYSTEM", false)]
  public void Given_Parameters_When_ctor_Then_CorrectSessionConstructed(string? actorIdValue, bool generateId)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);
    SessionId? id = generateId ? SessionId.NewId(realmId: null) : null;

    Base64Password secret = new(_faker.Internet.Password());
    Session session = new(_user, secret, actorId, id);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, session.Id);
    }
    else
    {
      Assert.Null(session.RealmId);
      Assert.NotEqual(Guid.Empty, session.EntityId);
    }

    Assert.Equal(actorId ?? new(_user.Id.Value), session.CreatedBy);
    Assert.Equal(_user.Id, session.UserId);
    Assert.True(session.IsPersistent);
    Assert.True(session.IsActive);
  }

  [Fact(DisplayName = "Delete: it should delete the session.")]
  public void Given_Session_When_Delete_Then_Deleted()
  {
    _session.Delete();
    Assert.True(_session.IsDeleted);

    _session.ClearChanges();
    _session.Delete();
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);
  }

  [Fact(DisplayName = "It should have the correct IDs.")]
  public void Given_Session_When_getIds_Then_CorrectIds()
  {
    SessionId id = new(Guid.NewGuid(), RealmId.NewId());
    Session session = new(_user, sessionId: id);
    Assert.Equal(id, session.Id);
    Assert.Equal(id.RealmId, session.RealmId);
    Assert.Equal(id.EntityId, session.EntityId);
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute.")]
  public void Given_CustomAttributes_When_RemoveCustomAttribute_Then_CustomAttributeRemoved()
  {
    Identifier key = new("IpAddress");
    _session.SetCustomAttribute(key, _faker.Internet.Ip());
    _session.Update();

    _session.RemoveCustomAttribute(key);
    _session.Update();
    Assert.False(_session.CustomAttributes.ContainsKey(key));
    Assert.Contains(_session.Changes, change => change is SessionUpdated updated && updated.CustomAttributes[key] == null);

    _session.ClearChanges();
    _session.RemoveCustomAttribute(key);
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);
  }

  [Theory(DisplayName = "Renew: it should renew a persistent session.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Secret_When_Renew_Then_Renewed(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    string oldSecretString = RandomStringGenerator.GetString();
    Base64Password oldSecret = new(oldSecretString);
    Session session = new(_user, oldSecret);

    string newSecretString = RandomStringGenerator.GetString();
    Base64Password newSecret = new(oldSecretString);
    session.Renew(oldSecretString, newSecret, actorId);

    Assert.True(session.IsPersistent);
    Assert.Contains(session.Changes, change => change is SessionRenewed renewed
      && renewed.Secret.Equals(newSecret)
      && renewed.ActorId == (actorId ?? new ActorId(_user.Id.Value)));
  }

  [Fact(DisplayName = "Renew: it should throw IncorrectSessionSecretException when the secret is not correct.")]
  public void Given_Incorrect_When_Renew_Then_IncorrectSessionSecretException()
  {
    Base64Password oldSecret = new(RandomStringGenerator.GetString());
    Session session = new(_user, oldSecret);

    string attemptedSecret = "incorrect";
    Base64Password newSecret = new(RandomStringGenerator.GetString());
    var exception = Assert.Throws<IncorrectSessionSecretException>(() => session.Renew(attemptedSecret, newSecret));
    Assert.Equal(session.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(session.EntityId, exception.SessionId);
    Assert.Equal(attemptedSecret, exception.AttemptedSecret);
  }

  [Fact(DisplayName = "Renew: it should throw SessionIsNotActiveException when the session is not active.")]
  public void Given_NotActive_When_Renew_Then_SessionIsNotActiveException()
  {
    _session.SignOut();

    var exception = Assert.Throws<SessionIsNotActiveException>(() => _session.Renew("secret", new Base64Password("secret")));
    Assert.Equal(_session.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_session.EntityId, exception.SessionId);
  }

  [Fact(DisplayName = "Renew: it should throw SessionIsNotPersistentException when the session is not persistent.")]
  public void Given_NotPersistent_When_Renew_Then_SessionIsNotPersistentException()
  {
    var exception = Assert.Throws<SessionIsNotPersistentException>(() => _session.Renew("secret", new Base64Password("secret")));
    Assert.Equal(_session.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_session.EntityId, exception.SessionId);
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("IpAddress");
    _session.SetCustomAttribute(key, _faker.Internet.Ip());
    _session.Update();

    _session.SetCustomAttribute(key, value!);
    _session.Update();
    Assert.False(_session.CustomAttributes.ContainsKey(key));
    Assert.Contains(_session.Changes, change => change is SessionUpdated updated && updated.CustomAttributes[key] == null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("IpAddress");
    string value = $"  {_faker.Internet.Ip()}  ";

    _session.SetCustomAttribute(key, value);
    _session.Update();
    Assert.Equal(_session.CustomAttributes[key], value.Trim());
    Assert.Contains(_session.Changes, change => change is SessionUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _session.ClearChanges();
    _session.SetCustomAttribute(key, value.Trim());
    _session.Update();
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);
  }

  [Theory(DisplayName = "SignOut: it should sign-out an active session.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Active_When_SignOut_Then_SignedOut(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _session.SignOut(actorId);
    Assert.False(_session.IsActive);
    Assert.Contains(_session.Changes, change => change is SessionSignedOut signedOut && signedOut.ActorId == (actorId ?? new(_user.Id.Value)));

    _session.ClearChanges();
    _session.SignOut();
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);
  }

  [Theory(DisplayName = "Update: it should update the session.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_SessionUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _session.ClearChanges();
    _session.Update();
    Assert.False(_session.HasChanges);
    Assert.Empty(_session.Changes);

    _session.SetCustomAttribute(new Identifier("IpAddress"), _faker.Internet.Ip());
    _session.Update(actorId);
    Assert.Contains(_session.Changes, change => change is SessionUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }
}
