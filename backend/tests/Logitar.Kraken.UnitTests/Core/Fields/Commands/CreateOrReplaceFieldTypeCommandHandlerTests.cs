using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Security.Cryptography;
using Moq;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceFieldTypeCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IFieldTypeManager> _fieldTypeManager = new();
  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly CreateOrReplaceFieldTypeCommandHandler _handler;

  public CreateOrReplaceFieldTypeCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _fieldTypeManager.Object, _fieldTypeQuerier.Object, _fieldTypeRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Theory(DisplayName = "It should create a new Boolean field type.")]
  [InlineData(null)]
  [InlineData("83128db6-4343-40a1-8c4b-de2668eb1700")]
  public async Task Given_BooleanNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured",
      DisplayName = " Is featured? ",
      Description = "  This is the field type for blog article feature marker.  ",
      Boolean = new BooleanPropertiesModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.EntityId == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.Boolean && y.Properties is BooleanProperties),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new DateTime field type.")]
  [InlineData(null)]
  [InlineData("eb62f571-505c-497f-8723-f74a017e4ca5")]
  public async Task Given_DateTimeNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "PublicationDate",
      DisplayName = " Publication Date ",
      Description = "  This is the field type for blog article original publication dates.  ",
      DateTime = new DateTimePropertiesModel
      {
        MinimumValue = new DateTime(2000, 1, 1),
        MaximumValue = new DateTime(2024, 12, 31, 23, 59, 59)
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.EntityId == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.DateTime
        && ((DateTimeProperties)y.Properties).MinimumValue == payload.DateTime.MinimumValue
        && ((DateTimeProperties)y.Properties).MaximumValue == payload.DateTime.MaximumValue),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new Number field type.")]
  [InlineData(null)]
  [InlineData("9989da9f-3d34-42a6-b283-e65195d7905d")]
  public async Task Given_NumberNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "WordCount",
      DisplayName = " Word Count ",
      Description = "  This is the field type for blog article word counts.  ",
      Number = new NumberPropertiesModel
      {
        MinimumValue = 1.0,
        Step = 1.0
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.EntityId == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.Number
        && ((NumberProperties)y.Properties).MinimumValue == payload.Number.MinimumValue
        && ((NumberProperties)y.Properties).MaximumValue == payload.Number.MaximumValue
        && ((NumberProperties)y.Properties).Step == payload.Number.Step),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new RelatedContent field type.")]
  [InlineData(null)]
  [InlineData("f8245cf4-b6de-4771-9349-ff07a3e681b5")]
  public async Task Given_RelatedContentNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleAuthor",
      DisplayName = " Article Author ",
      Description = "  This is the field type for blog article authors.  ",
      RelatedContent = new RelatedContentPropertiesModel
      {
        ContentTypeId = Guid.NewGuid(),
        IsMultiple = true
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.EntityId == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.RelatedContent
        && ((RelatedContentProperties)y.Properties).ContentTypeId.EntityId == payload.RelatedContent.ContentTypeId
        && ((RelatedContentProperties)y.Properties).IsMultiple == payload.RelatedContent.IsMultiple),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new RichText field type.")]
  [InlineData(null)]
  [InlineData("413e58aa-b2cb-4bd9-a5d4-7f0a89e2d9b6")]
  public async Task Given_RichTextNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleContent",
      DisplayName = " Article Content ",
      Description = "  This is the field type for blog article contents.  ",
      RichText = new RichTextPropertiesModel
      {
        ContentType = MediaTypeNames.Text.Plain,
        MinimumLength = 1
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.EntityId == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.RichText
        && ((RichTextProperties)y.Properties).ContentType == payload.RichText.ContentType
        && ((RichTextProperties)y.Properties).MinimumLength == payload.RichText.MinimumLength
        && ((RichTextProperties)y.Properties).MaximumLength == payload.RichText.MaximumLength),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new Select field type.")]
  [InlineData(null)]
  [InlineData("60746da6-36f6-4cdf-8992-4a7d244d192e")]
  public async Task Given_SelectNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleCategory",
      DisplayName = " Article Categtory ",
      Description = "  This is the field type for blog article categories.  ",
      Select = new SelectPropertiesModel
      {
        IsMultiple = true,
        Options =
        [
          new SelectOptionModel
          {
            Text = "Software Architecture",
            Value = "software-architecture"
          }
        ]
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.EntityId == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.Select
        && ((SelectProperties)y.Properties).IsMultiple == payload.Select.IsMultiple
        && ((SelectProperties)y.Properties).Options.Single().Equals(new SelectOption(payload.Select.Options.Single()))),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new String field type.")]
  [InlineData(null)]
  [InlineData("556d1ca3-3692-4600-89ff-1c22ced24d69")]
  public async Task Given_StringNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleTitle",
      DisplayName = " Article Title ",
      Description = "  This is the field type for blog article titles.  ",
      String = new StringPropertiesModel
      {
        MinimumLength = 1,
        MaximumLength = 100
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.EntityId == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.String
        && ((StringProperties)y.Properties).MinimumLength == payload.String.MinimumLength
        && ((StringProperties)y.Properties).MaximumLength == payload.String.MaximumLength
        && ((StringProperties)y.Properties).Pattern == payload.String.Pattern),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new Tags field type.")]
  [InlineData(null)]
  [InlineData("1731da09-02e3-4ce8-850c-106817127410")]
  public async Task Given_TagsNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleKeywords",
      DisplayName = " Article Keywords ",
      Description = "  This is the field type for blog article keywords.  ",
      Tags = new TagsPropertiesModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.EntityId == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.Tags && y.Properties is TagsProperties),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should replace/update an existing Boolean field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_BooleanExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "IsFeatured"), new BooleanProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Properties, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Featured");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured",
      DisplayName = " Is featured? ",
      Description = "  This is the field type for blog article feature marker.  ",
      Boolean = new BooleanPropertiesModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.EntityId, payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.Boolean, new BooleanPropertiesModel((BooleanProperties)fieldType.Properties));
  }

  [Theory(DisplayName = "It should replace/update an existing DateTime field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_DateTimeExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "PublicationDate"), new DateTimeProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Properties, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "PublishedOn");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "PublicationDate",
      DisplayName = " Publication Date ",
      Description = "  This is the field type for blog article original publication dates.  ",
      DateTime = new DateTimePropertiesModel
      {
        MinimumValue = new DateTime(2000, 1, 1),
        MaximumValue = new DateTime(2024, 12, 31, 23, 59, 59)
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.EntityId, payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.DateTime, new DateTimePropertiesModel((DateTimeProperties)fieldType.Properties));
  }

  [Theory(DisplayName = "It should replace/update an existing Number field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NumberExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "WordCount"), new NumberProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Properties, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Words");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "WordCount",
      DisplayName = " Word Count ",
      Description = "  This is the field type for blog article word counts.  ",
      Number = new NumberPropertiesModel
      {
        MinimumValue = 1.0,
        Step = 1.0
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.EntityId, payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.Number, new NumberPropertiesModel((NumberProperties)fieldType.Properties));
  }

  [Theory(DisplayName = "It should replace/update an existing RelatedContent field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_RelatedContentExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleAuthor"), new RelatedContentProperties(new ContentTypeId(), isMultiple: false));
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Properties, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Author");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleAuthor",
      DisplayName = " Article Author ",
      Description = "  This is the field type for blog article authors.  ",
      RelatedContent = new RelatedContentPropertiesModel
      {
        ContentTypeId = Guid.NewGuid(),
        IsMultiple = true
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.EntityId, payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.RelatedContent.ContentTypeId, ((RelatedContentProperties)fieldType.Properties).ContentTypeId.EntityId);
    Assert.Equal(payload.RelatedContent.IsMultiple, ((RelatedContentProperties)fieldType.Properties).IsMultiple);
  }

  [Theory(DisplayName = "It should replace/update an existing RichText field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_RichTextExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleContent"), new RichTextProperties(MediaTypeNames.Text.Plain));
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Properties, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Contents");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleContent",
      DisplayName = " Article Content ",
      Description = "  This is the field type for blog article contents.  ",
      RichText = new RichTextPropertiesModel
      {
        ContentType = MediaTypeNames.Text.Plain,
        MinimumLength = 1
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.EntityId, payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.RichText, new RichTextPropertiesModel((RichTextProperties)fieldType.Properties));
  }

  [Theory(DisplayName = "It should replace/update an existing Select field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_SelectExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleCategory"), new SelectProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Properties, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Category");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleCategory",
      DisplayName = " Article Categtory ",
      Description = "  This is the field type for blog article categories.  ",
      Select = new SelectPropertiesModel
      {
        IsMultiple = true,
        Options =
        [
          new SelectOptionModel
          {
            Text = "Software Architecture",
            Value = "software-architecture"
          },
          new SelectOptionModel
          {
            Text = "Linux Administration",
            Value = "linux-administration"
          }
        ]
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.EntityId, payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);

    SelectProperties? properties = fieldType.Properties as SelectProperties;
    Assert.NotNull(properties);
    Assert.Equal(payload.Select.IsMultiple, properties.IsMultiple);
    foreach (SelectOptionModel option in payload.Select.Options)
    {
      Assert.Contains(properties.Options, o => o.Text == option.Text && o.Value == option.Value);
    }
  }

  [Theory(DisplayName = "It should replace/update an existing String field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_StringExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Properties, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Title");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleTitle",
      DisplayName = " Article Title ",
      Description = "  This is the field type for blog article titles.  ",
      String = new StringPropertiesModel
      {
        MinimumLength = 1,
        MaximumLength = 100
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.EntityId, payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.String, new StringPropertiesModel((StringProperties)fieldType.Properties));
  }

  [Theory(DisplayName = "It should replace/update an existing Tags field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_TagsExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleKeywords"), new TagsProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Properties, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Keywords");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleKeywords",
      DisplayName = " Article Keywords ",
      Description = "  This is the field type for blog article keywords.  ",
      Tags = new TagsPropertiesModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.EntityId, payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.Tags, new TagsPropertiesModel((TagsProperties)fieldType.Properties));
  }

  [Fact(DisplayName = "It should return null when updating a field type that does not exist.")]
  public async Task Given_NotExistsWithVersion_When_Handle_Then_EmptyResult()
  {
    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured",
      Boolean = new BooleanPropertiesModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(Guid.NewGuid(), payload, Version: -1);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.Null(result.FieldType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured?",
      DisplayName = RandomStringGenerator.GetString(999),
      RichText = new RichTextPropertiesModel
      {
        ContentType = "text"
      },
      String = new StringPropertiesModel
      {
        MinimumLength = -1
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.PropertyName == "RichText.ContentType");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "String.MinimumLength.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CreateOrReplaceFieldTypeValidator");
  }
}
