﻿using Bogus;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.Core.Users.Events;

namespace Logitar.Kraken.Core;

[Trait(Traits.Category, Categories.Unit)]
public class CustomizableExtensionsTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "SetCustomAttributes: it should replace the custom attributes when there is no reference.")]
  public void Given_NoReference_When_SetCustomAttributes_Then_CustomAttributesReplaced()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    user.SetCustomAttribute(new Identifier("HealthInsuranceNumber"), _faker.Person.BuildHealthInsuranceNumber());
    user.SetCustomAttribute(new Identifier("SocialSecurityNumber"), _faker.Random.Number(0, 999999999).ToString());
    user.Update();
    user.ClearChanges();

    CustomAttributeModel[] customAttributes =
    [
      new CustomAttributeModel("HealthInsuranceNumber", "HealthInsuranceNumber"),
      new CustomAttributeModel("EmployeeId", Guid.NewGuid().ToString())
    ];
    int result = user.SetCustomAttributes(customAttributes);

    Assert.Equal(3, result);
    Assert.Equal(2, user.CustomAttributes.Count);
    foreach (CustomAttributeModel customAttribute in customAttributes)
    {
      Assert.Contains(user.CustomAttributes, c => c.Key.Value == customAttribute.Key && c.Value == customAttribute.Value);
    }
    Assert.True(user.HasChanges);
    Assert.Contains(user.Changes, change => change is UserUpdated updated && updated.CustomAttributes.Count == result);
  }

  [Fact(DisplayName = "SetCustomAttributes: it should return 0 when there is no change.")]
  public void Given_NoChange_When_SetCustomAttributes_Then_ZeroReturned()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    Identifier key = new("HealthInsuranceNumber");
    string value = _faker.Person.BuildHealthInsuranceNumber();
    user.SetCustomAttribute(key, value);
    user.Update();
    user.ClearChanges();

    int result = user.SetCustomAttributes([new CustomAttributeModel(key.Value, value)], user);

    Assert.Equal(0, result);
    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);
  }

  [Fact(DisplayName = "SetCustomAttributes: it should return a number when where are changes.")]
  public void Given_Changes_When_SetCustomAttributes_Then_NumberReturned()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    user.SetCustomAttribute(new Identifier("HealthInsuranceNumber"), "<unknown>");
    user.SetCustomAttribute(new Identifier("Salary"), "<unknown>");
    user.SetCustomAttribute(new Identifier("SocialSecurityNumber"), _faker.Random.Int(0, 999999999).ToString());

    User reference = new(user.UniqueName, user.CreatedBy, user.Id);
    foreach (KeyValuePair<Identifier, string> customAttribute in user.CustomAttributes)
    {
      reference.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    reference.Update();
    reference.ClearChanges();

    user.SetCustomAttribute(new Identifier("JobTitle"), "Software Developer");
    string salary = 100000.00.ToString();
    user.SetCustomAttribute(new Identifier("Salary"), salary);
    user.Update();
    user.ClearChanges();

    string healthInsuranceNumber = _faker.Person.BuildHealthInsuranceNumber();
    CustomAttributeModel[] customAttributes =
    [
      new CustomAttributeModel("Department", "Technology"),
      new CustomAttributeModel("HealthInsuranceNumber",healthInsuranceNumber),
      new CustomAttributeModel("Salary", "<unknown>")
    ];
    int result = user.SetCustomAttributes(customAttributes, reference);

    Assert.Equal(3, result);
    Assert.Equal(4, user.CustomAttributes.Count);
    Assert.Contains(user.CustomAttributes, c => c.Key.Value == "Department" && c.Value == "Technology");
    Assert.Contains(user.CustomAttributes, c => c.Key.Value == "HealthInsuranceNumber" && c.Value == healthInsuranceNumber);
    Assert.Contains(user.CustomAttributes, c => c.Key.Value == "JobTitle" && c.Value == "Software Developer");
    Assert.Contains(user.CustomAttributes, c => c.Key.Value == "Salary" && c.Value == salary);
  }
}
