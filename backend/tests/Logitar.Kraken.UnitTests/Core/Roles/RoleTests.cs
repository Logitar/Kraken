using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles.Events;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Core.Roles;

[Trait(Traits.Category, Categories.Unit)]
public class RoleTests
{
  private readonly Role _role;

  public RoleTests()
  {
    _role = new(new UniqueName(new UniqueNameSettings(), "admin"));
  }

  [Theory(DisplayName = "ctor: it should construct the correct role.")]
  [InlineData(null, true)]
  [InlineData("SYSTEM", false)]
  public void Given_Parameters_When_ctor_Then_CorrectRoleConstructed(string? actorIdValue, bool generateId)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);
    RoleId? id = generateId ? RoleId.NewId(realmId: null) : null;

    Role role = new(_role.UniqueName, actorId, id);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, role.Id);
    }
    else
    {
      Assert.Null(role.RealmId);
      Assert.NotEqual(Guid.Empty, role.EntityId);
    }

    Assert.Equal(actorId, role.CreatedBy);
    Assert.Equal(_role.UniqueName, role.UniqueName);
  }

  [Fact(DisplayName = "Delete: it should delete the role.")]
  public void Given_Role_When_Delete_Then_Deleted()
  {
    _role.Delete();
    Assert.True(_role.IsDeleted);

    _role.ClearChanges();
    _role.Delete();
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);
  }

  [Fact(DisplayName = "Description: it should handle the updates correctly.")]
  public void Given_DescriptionUpdates_When_setDescription_Then_UpdatesHandledCorrectly()
  {
    _role.ClearChanges();

    _role.Description = null;
    _role.Update();
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);

    _role.Description = new Description("This is a new role.");
    _role.Update();
    Assert.True(_role.HasChanges);
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.Description?.Value == _role.Description);
  }

  [Fact(DisplayName = "DisplayName: it should handle the updates correctly.")]
  public void Given_DisplayNameUpdates_When_setDisplayName_Then_UpdatesHandledCorrectly()
  {
    _role.ClearChanges();

    _role.DisplayName = _role.DisplayName;
    _role.Update();
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);

    _role.DisplayName = new DisplayName("New API Key");
    _role.Update();
    Assert.True(_role.HasChanges);
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.DisplayName?.Value == _role.DisplayName);
  }

  [Fact(DisplayName = "It should have the correct IDs.")]
  public void Given_Role_When_getIds_Then_CorrectIds()
  {
    RoleId id = new(Guid.NewGuid(), RealmId.NewId());
    Role role = new(_role.UniqueName, actorId: null, id);
    Assert.Equal(id, role.Id);
    Assert.Equal(id.RealmId, role.RealmId);
    Assert.Equal(id.EntityId, role.EntityId);
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute.")]
  public void Given_CustomAttributes_When_RemoveCustomAttribute_Then_CustomAttributeRemoved()
  {
    Identifier key = new("CanManageApi");
    _role.SetCustomAttribute(key, bool.TrueString);
    _role.Update();

    _role.RemoveCustomAttribute(key);
    _role.Update();
    Assert.False(_role.CustomAttributes.ContainsKey(key));
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.CustomAttributes[key] == null);

    _role.ClearChanges();
    _role.RemoveCustomAttribute(key);
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("CanManageApi");
    _role.SetCustomAttribute(key, bool.TrueString);
    _role.Update();

    _role.SetCustomAttribute(key, value!);
    _role.Update();
    Assert.False(_role.CustomAttributes.ContainsKey(key));
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.CustomAttributes[key] == null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("CanManageApi");
    string value = $"  {bool.TrueString}  ";

    _role.SetCustomAttribute(key, value);
    _role.Update();
    Assert.Equal(_role.CustomAttributes[key], value.Trim());
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _role.ClearChanges();
    _role.SetCustomAttribute(key, value.Trim());
    _role.Update();
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);
  }

  [Fact(DisplayName = "SetUniqueName: it should handle the updated correctly.")]
  public void Given_UniqueNameUpdates_When_setSetUniqueName_Then_UpdatesHandledCorrectly()
  {
    UniqueName uniqueName = new(new UniqueNameSettings(), "member");
    _role.SetUniqueName(uniqueName);
    Assert.Contains(_role.Changes, change => change is RoleUniqueNameChanged changed && changed.UniqueName == uniqueName);

    _role.ClearChanges();
    _role.SetUniqueName(uniqueName);
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(null)]
  [InlineData("Administrator")]
  public void Given_Role_When_ToString_Then_CorrectString(string? displayName)
  {
    if (displayName == null)
    {
      Assert.StartsWith(_role.UniqueName.Value, _role.ToString());
    }
    else
    {
      _role.DisplayName = new(displayName);
      Assert.StartsWith(_role.DisplayName.Value, _role.ToString());
    }
  }

  [Theory(DisplayName = "Update: it should update the role.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_RoleUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _role.ClearChanges();
    _role.Update();
    Assert.False(_role.HasChanges);
    Assert.Empty(_role.Changes);

    _role.SetCustomAttribute(new Identifier("CanManageApi"), bool.TrueString);
    _role.Update(actorId);
    Assert.Contains(_role.Changes, change => change is RoleUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }
}
