using Bogus;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Users;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class RecipientTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "It should construct the correct recipient from a user.")]
  public void Given_User_When_ctor_Then_Constructed()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName))
    {
      FirstName = new PersonName(_faker.Person.FirstName),
      LastName = new PersonName(_faker.Person.LastName)
    };
    user.SetEmail(new Email(_faker.Person.Email));

    Recipient recipient = new(user, RecipientType.CC);

    Assert.Equal(RecipientType.CC, recipient.Type);
    Assert.NotNull(user.Email);
    Assert.Equal(user.Email.Address, recipient.Address);
    Assert.Equal(user.FullName, recipient.DisplayName);
    Assert.Null(recipient.PhoneNumber);
    Assert.NotNull(recipient.UserId);
    Assert.Equal(user.Id, recipient.UserId);
    Assert.NotNull(recipient.User);
    Assert.Same(user, recipient.User);
  }

  [Fact(DisplayName = "It should construct the correct recipient from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    RecipientType type = RecipientType.Bcc;
    string phoneNumber = $"  {new Phone("+15148454636").FormatToE164()}  ";

    Recipient recipient = new(type, address: null, displayName: "    ", phoneNumber, userId: null);

    Assert.Equal(type, recipient.Type);
    Assert.Null(recipient.Address);
    Assert.Null(recipient.DisplayName);
    Assert.Equal(phoneNumber.Trim(), recipient.PhoneNumber);
    Assert.Null(recipient.UserId);
    Assert.Null(recipient.User);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    RecipientType type = (RecipientType)(-1);
    string address = "aa@@bb..cc";
    string displayName = RandomStringGenerator.GetString(999);
    string phoneNumber = "invalid";
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Recipient(type, address, displayName, phoneNumber));

    Assert.Equal(4, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Type");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EmailValidator" && e.PropertyName == "Address");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PhoneNumberValidator" && e.PropertyName == "PhoneNumber");
  }

  [Fact(DisplayName = "It should throw ValidationException when there is no contact information")]
  public void Given_NoContactInfo_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new Recipient());

    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RecipientValidator");
  }
}
