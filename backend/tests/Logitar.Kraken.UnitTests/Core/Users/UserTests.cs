using Bogus;
using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Users.Events;

namespace Logitar.Kraken.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class UserTests
{
  private readonly Faker _faker = new();

  private readonly User _user;

  public UserTests()
  {
    _user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
  }

  [Theory(DisplayName = "ctor: it should construct the correct user.")]
  [InlineData(null, true)]
  [InlineData("SYSTEM", false)]
  public void Given_Parameters_When_ctor_Then_CorrectUserConstructed(string? actorIdValue, bool generateId)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);
    UserId? id = generateId ? UserId.NewId(realmId: null) : null;

    Base64Password password = new("Test123!");
    User user = new(_user.UniqueName, password, actorId, id);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, user.Id);
    }
    else
    {
      Assert.Null(user.RealmId);
      Assert.NotEqual(Guid.Empty, user.EntityId);
    }

    Assert.Equal(actorId, user.CreatedBy);
    Assert.Equal(_user.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);

    FieldInfo? field = user.GetType().GetField("_password", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.NotNull(field);
    Password? instance = field.GetValue(user) as Password;
    Assert.NotNull(instance);
    Assert.Same(password, instance);
  }

  [Fact(DisplayName = "Delete: it should delete the user.")]
  public void Given_User_When_Delete_Then_Deleted()
  {
    _user.Delete();
    Assert.True(_user.IsDeleted);

    _user.ClearChanges();
    _user.Delete();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "It should have the correct IDs.")]
  public void Given_User_When_getIds_Then_CorrectIds()
  {
    UserId id = new(Guid.NewGuid(), RealmId.NewId());
    User user = new(_user.UniqueName, password: null, actorId: null, id);
    Assert.Equal(id, user.Id);
    Assert.Equal(id.RealmId, user.RealmId);
    Assert.Equal(id.EntityId, user.EntityId);
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute.")]
  public void Given_CustomAttributes_When_RemoveCustomAttribute_Then_CustomAttributeRemoved()
  {
    Identifier key = new("HealthInsuranceNumber");
    _user.SetCustomAttribute(key, _faker.Person.BuildHealthInsuranceNumber());
    _user.Update();

    _user.RemoveCustomAttribute(key);
    _user.Update();
    Assert.False(_user.CustomAttributes.ContainsKey(key));
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.CustomAttributes[key] == null);

    _user.ClearChanges();
    _user.RemoveCustomAttribute(key);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("HealthInsuranceNumber");
    _user.SetCustomAttribute(key, _faker.Person.BuildHealthInsuranceNumber());
    _user.Update();

    _user.SetCustomAttribute(key, value!);
    _user.Update();
    Assert.False(_user.CustomAttributes.ContainsKey(key));
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.CustomAttributes[key] == null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("HealthInsuranceNumber");
    string value = $"  {_faker.Person.BuildHealthInsuranceNumber()}  ";

    _user.SetCustomAttribute(key, value);
    _user.Update();
    Assert.Equal(_user.CustomAttributes[key], value.Trim());
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _user.ClearChanges();
    _user.SetCustomAttribute(key, value.Trim());
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "SetUniqueName: it should handle the updated correctly.")]
  public void Given_UniqueNameUpdates_When_setSetUniqueName_Then_UpdatesHandledCorrectly()
  {
    UniqueName uniqueName = new(new UniqueNameSettings(), "member");
    _user.SetUniqueName(uniqueName);
    Assert.Contains(_user.Changes, change => change is UserUniqueNameChanged changed && changed.UniqueName == uniqueName);

    _user.ClearChanges();
    _user.SetUniqueName(uniqueName);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Theory(DisplayName = "Update: it should update the user.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_UserUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _user.ClearChanges();
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.SetCustomAttribute(new Identifier("HealthInsuranceNumber"), _faker.Person.BuildHealthInsuranceNumber());
    _user.Update(actorId);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }
}
