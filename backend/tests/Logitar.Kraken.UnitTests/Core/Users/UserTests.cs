using Bogus;
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

  [Fact(DisplayName = "Birthdate: it should handle the updates correctly.")]
  public void Given_DisplayNameUpdates_When_setDisplayName_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.Birthdate = _user.Birthdate;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.Birthdate = _faker.Person.DateOfBirth;
    _user.Update();
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.Birthdate?.Value == _user.Birthdate);
  }

  [Fact(DisplayName = "Birthdate: it should throw ArgumentOutOfRangeException when the value is not in the past.")]
  public void Given_PresentOrFuture_When_setBirthdate_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _user.Birthdate = DateTime.Now.AddMinutes(1));
    Assert.StartsWith("The value must be a date and time set in the past.", exception.Message);
    Assert.Equal("Birthdate", exception.ParamName);
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
}
