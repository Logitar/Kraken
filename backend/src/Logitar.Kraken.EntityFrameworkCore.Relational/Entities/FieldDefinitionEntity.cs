using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class FieldDefinitionEntity
{
  public int FieldDefinitionId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid Id { get; private set; }
  public int Order { get; private set; }

  public FieldTypeEntity? FieldType { get; private set; }
  public int FieldTypeId { get; private set; }

  public bool IsInvariant { get; private set; }
  public bool IsRequired { get; private set; }
  public bool IsIndexed { get; private set; }
  public bool IsUnique { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }
  public string? Placeholder { get; private set; }

  public List<FieldIndexEntity> FieldIndex { get; private set; } = [];
  public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public FieldDefinitionEntity(ContentTypeEntity contentType, FieldTypeEntity fieldType, int order, ContentTypeFieldDefinitionChanged @event)
  {
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    Id = @event.FieldDefinition.Id;
    Order = order;

    FieldType = fieldType;
    FieldTypeId = fieldType.FieldTypeId;

    Update(@event);
  }

  private FieldDefinitionEntity()
  {
  }

  public void Update(ContentTypeFieldDefinitionChanged @event)
  {
    FieldDefinition field = @event.FieldDefinition;

    IsInvariant = field.IsInvariant;
    IsRequired = field.IsRequired;
    IsIndexed = field.IsIndexed;
    IsUnique = field.IsUnique;

    UniqueName = field.UniqueName.Value;
    DisplayName = field.DisplayName?.Value;
    Description = field.Description?.Value;
    Placeholder = field.Placeholder?.Value;
  }

  public override bool Equals(object? obj) => obj is FieldDefinitionEntity fieldDefinition
    && fieldDefinition.ContentTypeId == ContentTypeId
    && fieldDefinition.Id == Id;
  public override int GetHashCode() => HashCode.Combine(ContentTypeId, Id);
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (ContentTypeId={ContentTypeId}, Id={Id})";
}
