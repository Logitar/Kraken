using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Events;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Realms;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Fields;

[Trait(Traits.Category, Categories.Unit)]
public class FieldTypeTests
{
  private readonly FieldType _boolean = new(new UniqueName(FieldType.UniqueNameSettings, "IsFeatured"), new BooleanProperties());
  private readonly FieldType _dateTime = new(new UniqueName(FieldType.UniqueNameSettings, "PublicationDate"), new DateTimeProperties());
  private readonly FieldType _number = new(new UniqueName(FieldType.UniqueNameSettings, "Price"), new NumberProperties());
  private readonly FieldType _relatedContent = new(new UniqueName(FieldType.UniqueNameSettings, "ProductCategory"), new RelatedContentProperties(ContentTypeId.NewId(realmId: null)));
  private readonly FieldType _richText = new(new UniqueName(FieldType.UniqueNameSettings, "Contents"), new RichTextProperties(MediaTypeNames.Text.Plain));
  private readonly FieldType _select = new(new UniqueName(FieldType.UniqueNameSettings, "Availability"), new SelectProperties());
  private readonly FieldType _string = new(new UniqueName(FieldType.UniqueNameSettings, "Title"), new StringProperties());
  private readonly FieldType _tags = new(new UniqueName(FieldType.UniqueNameSettings, "Keywords"), new TagsProperties());

  [Fact(DisplayName = "Delete: it should delete the field type.")]
  public void Given_FieldType_When_Delete_Then_Deleted()
  {
    _select.Delete();
    Assert.True(_select.IsDeleted);

    _select.ClearChanges();
    _select.Delete();
    Assert.False(_select.HasChanges);
    Assert.Empty(_select.Changes);
  }

  [Fact(DisplayName = "Description: it should handle the updates correctly.")]
  public void Given_DescriptionUpdates_When_setDescription_Then_UpdatesHandledCorrectly()
  {
    _string.ClearChanges();

    _string.Description = null;
    _string.Update();
    Assert.False(_string.HasChanges);
    Assert.Empty(_string.Changes);

    _string.Description = new Description("This is the field type for product titles.");
    _string.Update();
    Assert.True(_string.HasChanges);
    Assert.Contains(_string.Changes, change => change is FieldTypeUpdated updated && updated.Description?.Value == _string.Description);
  }

  [Fact(DisplayName = "DisplayName: it should handle the updates correctly.")]
  public void Given_DisplayNameUpdates_When_setDisplayName_Then_UpdatesHandledCorrectly()
  {
    _richText.ClearChanges();

    _richText.DisplayName = _richText.DisplayName;
    _richText.Update();
    Assert.False(_richText.HasChanges);
    Assert.Empty(_richText.Changes);

    _richText.DisplayName = new DisplayName("Product Title");
    _richText.Update();
    Assert.True(_richText.HasChanges);
    Assert.Contains(_richText.Changes, change => change is FieldTypeUpdated updated && updated.DisplayName?.Value == _richText.DisplayName);
  }

  [Fact(DisplayName = "It should construct the correct instance.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    UniqueName uniqueName = new(FieldType.UniqueNameSettings, "Price");
    NumberProperties properties = new(minimumValue: 0.01, maximumValue: null, step: 0.01);
    ActorId actorId = ActorId.NewId();
    RealmId realmId = RealmId.NewId();
    Guid entityId = Guid.NewGuid();
    FieldTypeId id = new(realmId, entityId);

    FieldType fieldType = new(uniqueName, properties, actorId, id);

    Assert.Equal(actorId, fieldType.CreatedBy);
    Assert.Equal(actorId, fieldType.UpdatedBy);
    Assert.Equal(id, fieldType.Id);
    Assert.Equal(realmId, fieldType.RealmId);
    Assert.Equal(entityId, fieldType.EntityId);
    Assert.Equal(uniqueName, fieldType.UniqueName);
    Assert.Equal(DataType.Number, fieldType.DataType);
    Assert.Same(properties, fieldType.Properties);
  }

  [Fact(DisplayName = "It should throw DataTypeNotSupportedException when the data type is not supported.")]
  public void Given_NotSupported_When_ctor_Then_DataTypeNotSupportedException()
  {
    InvalidProperties properties = new();
    var exception = Assert.Throws<DataTypeNotSupportedException>(() => new FieldType(_boolean.UniqueName, properties));
    Assert.Equal(properties.DataType, exception.DataType);
  }

  [Fact(DisplayName = "It should handle BooleanProperties changes correctly.")]
  public void Given_Boolean_When_SetProperties_Then_ChangesHandled()
  {
    _boolean.ClearChanges();
    _boolean.SetProperties(new BooleanProperties());
    Assert.False(_boolean.HasChanges);
    Assert.Empty(_boolean.Changes);
  }

  [Fact(DisplayName = "It should handle DateTimeProperties changes correctly.")]
  public void Given_DateTime_When_SetProperties_Then_ChangesHandled()
  {
    DateTimeProperties properties = new(minimumValue: new DateTime(2000, 1, 1), maximumValue: new DateTime(2019, 12, 31, 23, 59, 59));
    ActorId actorId = ActorId.NewId();
    _dateTime.SetProperties(properties, actorId);
    Assert.Same(properties, _dateTime.Properties);
    Assert.Contains(_dateTime.Changes, change => change is FieldTypeDateTimePropertiesChanged changed && changed.ActorId == actorId && changed.Properties.Equals(properties));

    _dateTime.ClearChanges();
    _dateTime.SetProperties(properties, actorId);
    Assert.False(_dateTime.HasChanges);
    Assert.Empty(_dateTime.Changes);
  }

  [Fact(DisplayName = "It should handle NumberProperties changes correctly.")]
  public void Given_Number_When_SetProperties_Then_ChangesHandled()
  {
    NumberProperties properties = new(minimumValue: 0.00, maximumValue: null, step: 0.01);
    ActorId actorId = ActorId.NewId();
    _number.SetProperties(properties, actorId);
    Assert.Same(properties, _number.Properties);
    Assert.Contains(_number.Changes, change => change is FieldTypeNumberPropertiesChanged changed && changed.ActorId == actorId && changed.Properties.Equals(properties));

    _number.ClearChanges();
    _number.SetProperties(properties, actorId);
    Assert.False(_number.HasChanges);
    Assert.Empty(_number.Changes);
  }

  [Fact(DisplayName = "It should handle RelatedContentProperties changes correctly.")]
  public void Given_RelatedContent_When_SetProperties_Then_ChangesHandled()
  {
    RelatedContentProperties properties = new(ContentTypeId.NewId(realmId: null), isMultiple: true);
    ActorId actorId = ActorId.NewId();
    _relatedContent.SetProperties(properties, actorId);
    Assert.Same(properties, _relatedContent.Properties);
    Assert.Contains(_relatedContent.Changes, change => change is FieldTypeRelatedContentPropertiesChanged changed && changed.ActorId == actorId && changed.Properties.Equals(properties));

    _relatedContent.ClearChanges();
    _relatedContent.SetProperties(properties, actorId);
    Assert.False(_relatedContent.HasChanges);
    Assert.Empty(_relatedContent.Changes);
  }

  [Fact(DisplayName = "It should handle RichTextProperties changes correctly.")]
  public void Given_RichText_When_SetProperties_Then_ChangesHandled()
  {
    RichTextProperties properties = new(MediaTypeNames.Text.Plain, minimumLength: 1, maximumLength: 9999);
    ActorId actorId = ActorId.NewId();
    _richText.SetProperties(properties, actorId);
    Assert.Same(properties, _richText.Properties);
    Assert.Contains(_richText.Changes, change => change is FieldTypeRichTextPropertiesChanged changed && changed.ActorId == actorId && changed.Properties.Equals(properties));

    _richText.ClearChanges();
    _richText.SetProperties(properties, actorId);
    Assert.False(_richText.HasChanges);
    Assert.Empty(_richText.Changes);
  }

  [Fact(DisplayName = "It should handle SelectProperties changes correctly.")]
  public void Given_Select_When_SetProperties_Then_ChangesHandled()
  {
    SelectProperties properties = new(isMultiple: false, [new SelectOption("Available", value: "true"), new SelectOption("Unavailable", value: "false")]);
    ActorId actorId = ActorId.NewId();
    _select.SetProperties(properties, actorId);
    Assert.Same(properties, _select.Properties);
    Assert.Contains(_select.Changes, change => change is FieldTypeSelectPropertiesChanged changed && changed.ActorId == actorId && changed.Properties.Equals(properties));

    _select.ClearChanges();
    _select.SetProperties(properties, actorId);
    Assert.False(_select.HasChanges);
    Assert.Empty(_select.Changes);
  }

  [Fact(DisplayName = "It should handle StringProperties changes correctly.")]
  public void Given_String_When_SetProperties_Then_ChangesHandled()
  {
    StringProperties properties = new(minimumLength: 12, maximumLength: 14, pattern: "[A-Z]{4}\\s?[0-9]{4}\\s?[0-9]{4}");
    ActorId actorId = ActorId.NewId();
    _string.SetProperties(properties, actorId);
    Assert.Same(properties, _string.Properties);
    Assert.Contains(_string.Changes, change => change is FieldTypeStringPropertiesChanged changed && changed.ActorId == actorId && changed.Properties.Equals(properties));

    _string.ClearChanges();
    _string.SetProperties(properties, actorId);
    Assert.False(_string.HasChanges);
    Assert.Empty(_string.Changes);
  }

  [Fact(DisplayName = "It should handle TagsProperties changes correctly.")]
  public void Given_Tags_When_SetProperties_Then_ChangesHandled()
  {
    _tags.ClearChanges();
    _tags.SetProperties(new TagsProperties());
    Assert.False(_tags.HasChanges);
    Assert.Empty(_tags.Changes);
  }

  [Fact(DisplayName = "SetProperties: it should throw UnexpectedFieldTypePropertiesException when the data type was not expected.")]
  public void Given_DifferentDataType_When_SetProperties_Then_UnexpectedFieldTypePropertiesException()
  {
    var exception = Assert.Throws<UnexpectedFieldTypePropertiesException>(() => _boolean.SetProperties(new DateTimeProperties()));
    AssertException(exception, _boolean, DataType.DateTime);

    exception = Assert.Throws<UnexpectedFieldTypePropertiesException>(() => _dateTime.SetProperties(new BooleanProperties()));
    AssertException(exception, _dateTime, DataType.Boolean);

    exception = Assert.Throws<UnexpectedFieldTypePropertiesException>(() => _number.SetProperties(new BooleanProperties()));
    AssertException(exception, _number, DataType.Boolean);

    exception = Assert.Throws<UnexpectedFieldTypePropertiesException>(() => _relatedContent.SetProperties(new BooleanProperties()));
    AssertException(exception, _relatedContent, DataType.Boolean);

    exception = Assert.Throws<UnexpectedFieldTypePropertiesException>(() => _richText.SetProperties(new BooleanProperties()));
    AssertException(exception, _richText, DataType.Boolean);

    exception = Assert.Throws<UnexpectedFieldTypePropertiesException>(() => _select.SetProperties(new BooleanProperties()));
    AssertException(exception, _select, DataType.Boolean);

    exception = Assert.Throws<UnexpectedFieldTypePropertiesException>(() => _string.SetProperties(new BooleanProperties()));
    AssertException(exception, _string, DataType.Boolean);

    exception = Assert.Throws<UnexpectedFieldTypePropertiesException>(() => _tags.SetProperties(new BooleanProperties()));
    AssertException(exception, _tags, DataType.Boolean);
  }

  [Fact(DisplayName = "SetUniqueName: it should handle the updated correctly.")]
  public void Given_UniqueNameUpdates_When_setSetUniqueName_Then_UpdatesHandledCorrectly()
  {
    UniqueName uniqueName = new(FieldType.UniqueNameSettings, "PublishedOn");
    _dateTime.SetUniqueName(uniqueName);
    Assert.Contains(_dateTime.Changes, change => change is FieldTypeUniqueNameChanged changed && changed.UniqueName == uniqueName);

    _dateTime.ClearChanges();
    _dateTime.SetUniqueName(uniqueName);
    Assert.False(_dateTime.HasChanges);
    Assert.Empty(_dateTime.Changes);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(null)]
  [InlineData("Keywords (meta)")]
  public void Given_FieldType_When_ToString_Then_CorrectString(string? displayName)
  {
    if (displayName == null)
    {
      Assert.StartsWith(_tags.UniqueName.Value, _tags.ToString());
    }
    else
    {
      _tags.DisplayName = new(displayName);
      Assert.StartsWith(_tags.DisplayName.Value, _tags.ToString());
    }
  }

  [Theory(DisplayName = "Update: it should update the field type.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_FieldTypeUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _relatedContent.ClearChanges();
    _relatedContent.Update();
    Assert.False(_relatedContent.HasChanges);
    Assert.Empty(_relatedContent.Changes);

    _relatedContent.DisplayName = new DisplayName("Product Category");
    _relatedContent.Update(actorId);
    Assert.Contains(_relatedContent.Changes, change => change is FieldTypeUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }

  private static void AssertException(UnexpectedFieldTypePropertiesException exception, FieldType fieldType, DataType dataType)
  {
    Assert.Equal(fieldType.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(fieldType.EntityId, exception.FieldTypeId);
    Assert.Equal(fieldType.DataType, exception.ExpectedDataType);
    Assert.Equal(dataType, exception.ActualDataType);
  }

  private record InvalidProperties : FieldTypeProperties
  {
    public override DataType DataType { get; } = (DataType)(-1);
  }
}
