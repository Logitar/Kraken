using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeTests
{
  private readonly ContentType _contentType = new(new Identifier("BlogArticle"), isInvariant: false);

  [Fact(DisplayName = "Delete: it should delete the content.")]
  public void Given_Content_When_Delete_Then_Deleted()
  {
    _contentType.Delete();
    Assert.True(_contentType.IsDeleted);

    _contentType.ClearChanges();
    _contentType.Delete();
    Assert.False(_contentType.HasChanges);
    Assert.Empty(_contentType.Changes);
  }

  [Fact(DisplayName = "Description: it should handle the updates correctly.")]
  public void Given_DescriptionUpdates_When_setDescription_Then_UpdatesHandledCorrectly()
  {
    _contentType.ClearChanges();

    _contentType.Description = null;
    _contentType.Update();
    Assert.False(_contentType.HasChanges);
    Assert.Empty(_contentType.Changes);

    _contentType.Description = new Description("This is the content type for blog articles.");
    _contentType.Update();
    Assert.True(_contentType.HasChanges);
    Assert.Contains(_contentType.Changes, change => change is ContentTypeUpdated updated && updated.Description?.Value == _contentType.Description);
  }

  [Fact(DisplayName = "DisplayName: it should handle the updates correctly.")]
  public void Given_DisplayNameUpdates_When_setDisplayName_Then_UpdatesHandledCorrectly()
  {
    _contentType.ClearChanges();

    _contentType.DisplayName = _contentType.DisplayName;
    _contentType.Update();
    Assert.False(_contentType.HasChanges);
    Assert.Empty(_contentType.Changes);

    _contentType.DisplayName = new DisplayName("New API Key");
    _contentType.Update();
    Assert.True(_contentType.HasChanges);
    Assert.Contains(_contentType.Changes, change => change is ContentTypeUpdated updated && updated.DisplayName?.Value == _contentType.DisplayName);
  }

  [Fact(DisplayName = "TryGetField: it should return the field found by ID.")]
  public void Given_FoundById_When_FindField_Then_FieldReturned()
  {
    FieldDefinition title = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false, new Identifier("Title"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(title);

    FieldDefinition contents = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: false, IsUnique: false, new Identifier("Contents"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(contents);

    FieldDefinition field = _contentType.FindField(title.Id);
    Assert.Same(title, field);
  }

  [Fact(DisplayName = "FindField: it should return the field found by unique name.")]
  public void Given_FoundByUniqueName_When_FindField_Then_FieldReturned()
  {
    FieldDefinition title = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false, new Identifier("Title"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(title);

    FieldDefinition contents = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: false, IsUnique: false, new Identifier("Contents"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(contents);

    FieldDefinition field = _contentType.FindField(contents.UniqueName);
    Assert.Same(contents, field);
  }

  [Fact(DisplayName = "FindField: it should throw InvalidOperationException when the field was not found.")]
  public void Given_NotFound_When_FindField_Then_InvalidOperationException()
  {
    FieldDefinition title = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false, new Identifier("Title"), DisplayName: null, Description: null, Placeholder: null);
    FieldDefinition contents = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: false, IsUnique: false, new Identifier("Contents"), DisplayName: null, Description: null, Placeholder: null);

    var exception = Assert.Throws<InvalidOperationException>(() => _contentType.FindField(title.Id));
    Assert.Equal(string.Format("The field 'Id={0}' could not be found.", title.Id), exception.Message);

    exception = Assert.Throws<InvalidOperationException>(() => _contentType.FindField(contents.UniqueName));
    Assert.Equal(string.Format("The field 'UniqueName={0}' could not be found.", contents.UniqueName), exception.Message);
  }

  [Fact(DisplayName = "It should construct the correct instance.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    ActorId actorId = ActorId.NewId();
    RealmId realmId = RealmId.NewId();
    Guid entityId = Guid.NewGuid();
    ContentTypeId id = new(realmId, entityId);

    ContentType contentType = new(_contentType.UniqueName, _contentType.IsInvariant, actorId, id);

    Assert.Equal(actorId, contentType.CreatedBy);
    Assert.Equal(actorId, contentType.UpdatedBy);
    Assert.Equal(id, contentType.Id);
    Assert.Equal(realmId, contentType.RealmId);
    Assert.Equal(entityId, contentType.EntityId);
    Assert.Equal(_contentType.IsInvariant, contentType.IsInvariant);
    Assert.Equal(_contentType.UniqueName, contentType.UniqueName);
  }

  [Fact(DisplayName = "RemoveField: it should remove an existing field and return true.")]
  public void Given_Found_When_RemoveField_Then_RemovedAndTrue()
  {
    FieldDefinition fieldDefinition = new(
      Guid.NewGuid(),
      FieldTypeId.NewId(realmId: null),
      IsInvariant: false,
      IsRequired: false,
      IsIndexed: false,
      IsUnique: false,
      new Identifier("ArticleTitle"),
      DisplayName: null,
      Description: null,
      Placeholder: null);
    _contentType.SetField(fieldDefinition);

    ActorId actorId = ActorId.NewId();
    _contentType.RemoveField(fieldDefinition, actorId);

    Assert.Null(_contentType.TryGetField(fieldDefinition.Id));
    Assert.Contains(_contentType.Changes, change => change is ContentTypeFieldDefinitionRemoved removed && removed.FieldId == fieldDefinition.Id && removed.ActorId == actorId);
  }

  [Fact(DisplayName = "RemoveField: it should return false when the field was not found.")]
  public void Given_NotFound_When_RemoveField_Then_FalseReturned()
  {
    Assert.False(_contentType.RemoveField(Guid.NewGuid()));
  }

  [Fact(DisplayName = "SetField: it should not do anything when the field definition had no change.")]
  public void Given_FieldDefinitionNoChange_When_SetField_Then_NoChange()
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(minimumLength: 1, maximumLength: 100));
    FieldDefinition field1 = new(Guid.NewGuid(), fieldType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new Identifier("ArticleTitle"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(field1);
    _contentType.ClearChanges();

    FieldDefinition field2 = new(field1.Id, field1.FieldTypeId, field1.IsInvariant, field1.IsRequired, field1.IsIndexed, field1.IsUnique,
      field1.UniqueName, field1.DisplayName, field1.Description, field1.Placeholder);
    _contentType.SetField(field2);
    Assert.False(_contentType.HasChanges);
    Assert.Empty(_contentType.Changes);
  }

  [Fact(DisplayName = "SetField: it should throw UniqueNameAlreadyUsedException when the field definition unique name is already used.")]
  public void Given_FieldDefinitionNameConflict_When_SetField_Then_UniqueNameAlreadyUsedException()
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties(minimumLength: 1, maximumLength: 100));
    FieldDefinition conflict = new(Guid.NewGuid(), fieldType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new Identifier("ArticleTitle"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(conflict);

    FieldDefinition field = new(Guid.NewGuid(), conflict.FieldTypeId, conflict.IsInvariant, conflict.IsRequired, conflict.IsIndexed, conflict.IsUnique,
      conflict.UniqueName, conflict.DisplayName, conflict.Description, conflict.Placeholder);
    var exception = Assert.Throws<UniqueNameAlreadyUsedException>(() => _contentType.SetField(field));
    Assert.Equal(_contentType.RealmId?.ToGuid(), exception.RealmId);
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

  [Fact(DisplayName = "SetUniqueName: it should handle the updated correctly.")]
  public void Given_UniqueNameUpdates_When_setSetUniqueName_Then_UpdatesHandledCorrectly()
  {
    Identifier uniqueName = new("ArticleDeBlogue");
    _contentType.SetUniqueName(uniqueName);
    Assert.Contains(_contentType.Changes, change => change is ContentTypeUniqueNameChanged changed && changed.UniqueName == uniqueName);

    _contentType.ClearChanges();
    _contentType.SetUniqueName(uniqueName);
    Assert.False(_contentType.HasChanges);
    Assert.Empty(_contentType.Changes);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(null)]
  [InlineData("Blog Article")]
  public void Given_ContentType_When_ToString_Then_CorrectString(string? displayName)
  {
    if (displayName == null)
    {
      Assert.StartsWith(_contentType.UniqueName.Value, _contentType.ToString());
    }
    else
    {
      _contentType.DisplayName = new(displayName);
      Assert.StartsWith(_contentType.DisplayName.Value, _contentType.ToString());
    }
  }

  [Fact(DisplayName = "TryGetField: it should return null when the field was not found.")]
  public void Given_NotFound_When_TryGetField_Then_NullReturned()
  {
    FieldDefinition title = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false, new Identifier("Title"), DisplayName: null, Description: null, Placeholder: null);
    FieldDefinition contents = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: false, IsUnique: false, new Identifier("Contents"), DisplayName: null, Description: null, Placeholder: null);

    Assert.Null(_contentType.TryGetField(title.Id));
    Assert.Null(_contentType.TryGetField(contents.UniqueName));
  }

  [Fact(DisplayName = "TryGetField: it should return the field found by ID.")]
  public void Given_FoundById_When_TryGetField_Then_FieldReturned()
  {
    FieldDefinition title = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false, new Identifier("Title"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(title);

    FieldDefinition contents = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: false, IsUnique: false, new Identifier("Contents"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(contents);

    FieldDefinition? field = _contentType.TryGetField(title.Id);
    Assert.NotNull(field);
    Assert.Same(title, field);
  }

  [Fact(DisplayName = "TryGetField: it should return the field found by unique name.")]
  public void Given_FoundByUniqueName_When_TryGetField_Then_FieldReturned()
  {
    FieldDefinition title = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false, new Identifier("Title"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(title);

    FieldDefinition contents = new(Guid.NewGuid(), FieldTypeId.NewId(realmId: null), IsInvariant: false, IsRequired: true, IsIndexed: false, IsUnique: false, new Identifier("Contents"), DisplayName: null, Description: null, Placeholder: null);
    _contentType.SetField(contents);

    FieldDefinition? field = _contentType.TryGetField(contents.UniqueName);
    Assert.NotNull(field);
    Assert.Same(contents, field);
  }

  [Theory(DisplayName = "Update: it should update the content type.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_ContentTypeUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _contentType.ClearChanges();
    _contentType.Update();
    Assert.False(_contentType.HasChanges);
    Assert.Empty(_contentType.Changes);

    _contentType.IsInvariant = !_contentType.IsInvariant;
    _contentType.Update(actorId);
    Assert.Contains(_contentType.Changes, change => change is ContentTypeUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }
}
