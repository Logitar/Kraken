using FluentValidation;
using FluentValidation.Results;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Fields.Properties;

namespace Logitar.Kraken.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeTests
{
  [Fact(DisplayName = "Delete: it should delete the content.")]
  public void Given_Content_When_Delete_Then_Deleted()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);

    contentType.Delete();
    Assert.True(contentType.IsDeleted);

    contentType.ClearChanges();
    contentType.Delete();
    Assert.False(contentType.HasChanges);
    Assert.Empty(contentType.Changes);
  }

  [Fact(DisplayName = "SetField: it should not do anything when the field definition had no change.")]
  public void Given_FieldDefinitionNoChange_When_SetField_Then_NoChange()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);

    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(minimumLength: 1, maximumLength: 100));
    FieldDefinition field1 = new(Guid.NewGuid(), fieldType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new Identifier("ArticleTitle"), DisplayName: null, Description: null, Placeholder: null);
    contentType.SetField(field1);
    contentType.ClearChanges();

    FieldDefinition field2 = new(field1.Id, field1.FieldTypeId, field1.IsInvariant, field1.IsRequired, field1.IsIndexed, field1.IsUnique,
      field1.UniqueName, field1.DisplayName, field1.Description, field1.Placeholder);
    contentType.SetField(field2);
    Assert.False(contentType.HasChanges);
    Assert.Empty(contentType.Changes);
  }

  [Fact(DisplayName = "SetField: it should throw UniqueNameAlreadyUsedException when the field definition unique name is already used.")]
  public void Given_FieldDefinitionNameConflict_When_SetField_Then_UniqueNameAlreadyUsedException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);

    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(minimumLength: 1, maximumLength: 100));
    FieldDefinition conflict = new(Guid.NewGuid(), fieldType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new Identifier("ArticleTitle"), DisplayName: null, Description: null, Placeholder: null);
    contentType.SetField(conflict);

    FieldDefinition field = new(Guid.NewGuid(), conflict.FieldTypeId, conflict.IsInvariant, conflict.IsRequired, conflict.IsIndexed, conflict.IsUnique,
      conflict.UniqueName, conflict.DisplayName, conflict.Description, conflict.Placeholder);
    var exception = Assert.Throws<UniqueNameAlreadyUsedException>(() => contentType.SetField(field));
    Assert.Equal(contentType.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(field.Id, exception.ConflictId);
    Assert.Equal(conflict.Id, exception.EntityId);
    Assert.Equal(field.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "SetField: it should throw ValidationException when setting a variant field definition on an invariant content type.")]
  public void Given_InvariantContentTypeVariantFieldDefinition_When_SetField_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"));

    FieldDefinition field = new(
      Guid.NewGuid(),
      FieldTypeId.NewId(realmId: null),
      IsInvariant: false,
      IsRequired: false,
      IsIndexed: false,
      IsUnique: false,
      new Identifier("Biography"),
      DisplayName: null,
      Description: null,
      Placeholder: null);
    var exception = Assert.Throws<ValidationException>(() => contentType.SetField(field));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal(field.IsInvariant, failure.AttemptedValue);
    Assert.Equal("InvariantValidator", failure.ErrorCode);
    Assert.Equal("'IsInvariant' must be true. Invariant content types cannot define variant fields.", failure.ErrorMessage);
    Assert.Equal("IsInvariant", failure.PropertyName);
  }
}
