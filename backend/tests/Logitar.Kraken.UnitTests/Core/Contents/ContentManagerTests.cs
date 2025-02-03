using Bogus;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Fields.Validators;
using Logitar.Kraken.Core.Localization;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentManagerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();
  private readonly Mock<IFieldValueValidatorFactory> _fieldValueValidatorFactory = new();

  private readonly ContentManager _manager;

  private readonly ContentType _articleType = new(new Identifier("BlogArticle"), isInvariant: false);
  private readonly ContentType _authorType = new(new Identifier("BlogAuthor"));

  private readonly FieldType _emailAddressType = new(new UniqueName(FieldType.UniqueNameSettings, "EmailAddress"), new StringProperties());
  private readonly FieldType _personNameType = new(new UniqueName(FieldType.UniqueNameSettings, "PersonName"), new StringProperties(minimumLength: 2, maximumLength: 100));
  private readonly FieldDefinition _emailAddress;
  private readonly FieldDefinition _firstName;
  private readonly FieldDefinition _lastName;

  public ContentManagerTests()
  {
    _manager = new(_contentQuerier.Object, _contentRepository.Object, _contentTypeRepository.Object, _fieldTypeRepository.Object, _fieldValueValidatorFactory.Object);

    _emailAddress = new(Guid.NewGuid(), _emailAddressType.Id, IsInvariant: true, IsRequired: false, IsIndexed: true, IsUnique: true,
      new Identifier("EmailAddress"), new DisplayName("Email Address"), Description: null, new Placeholder("Enter the email address of the author."));
    _authorType.SetField(_emailAddress);

    _firstName = new(Guid.NewGuid(), _personNameType.Id, IsInvariant: true, IsRequired: true, IsIndexed: false, IsUnique: false,
      new Identifier("FirstName"), new DisplayName("First Name"), Description: null, new Placeholder("Enter the first name of the author."));
    _authorType.SetField(_firstName);

    _lastName = new(Guid.NewGuid(), _personNameType.Id, IsInvariant: true, IsRequired: true, IsIndexed: false, IsUnique: false,
      new Identifier("LastName"), new DisplayName("Last Name"), Description: null, new Placeholder("Enter the last name of the author."));
    _authorType.SetField(_lastName);
  }

  [Fact(DisplayName = "It should load the content type when it is not provided.")]
  public async Task Given_NoContentType_When_SaveAsync_Then_Loaded()
  {
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName));
    Content content = new(_authorType, invariant);
    content.ClearChanges();
    _contentTypeRepository.Setup(x => x.LoadAsync(content, _cancellationToken)).ReturnsAsync(_authorType);

    await _manager.SaveAsync(content, _cancellationToken);

    _contentRepository.Verify(x => x.SaveAsync(content, _cancellationToken), Times.Once);
    _contentTypeRepository.Verify(x => x.LoadAsync(content, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should save contents.")]
  public async Task Given_NoChange_When_SaveAsync_Then_Saved()
  {
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName));
    Content content = new(_authorType, invariant);
    content.ClearChanges();

    await _manager.SaveAsync(content, _authorType, _cancellationToken);

    _contentTypeRepository.Verify(x => x.LoadAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()), Times.Never);
    _fieldTypeRepository.Verify(x => x.LoadAsync(It.IsAny<IEnumerable<FieldTypeId>>(), It.IsAny<CancellationToken>()), Times.Never);
    _contentQuerier.Verify(x => x.FindIdAsync(It.IsAny<ContentTypeId>(), It.IsAny<LanguageId?>(), It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _fieldValueValidatorFactory.Verify(x => x.Create(It.IsAny<FieldType>()), Times.Never);

    _contentRepository.Verify(x => x.SaveAsync(content, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should save contents when the field values are valid.")]
  public async Task Given_ValidFieldValues_When_SaveAsync_Then_Saved()
  {
    FieldTypeId[] fieldTypeIds = [_emailAddressType.Id, _personNameType.Id];
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(fieldTypeIds)), _cancellationToken))
      .ReturnsAsync([_personNameType]);

    _fieldValueValidatorFactory.Setup(x => x.Create(_personNameType)).Returns(new StringValueValidator((StringProperties)_personNameType.Properties));

    ContentLocale invariant = new(
      new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName),
      new DisplayName(_faker.Person.FullName),
      description: null,
      fieldValues: new Dictionary<Guid, string>
      {
        [_firstName.Id] = _faker.Person.FirstName,
        [_lastName.Id] = _faker.Person.LastName
      });
    Content content = new(_authorType, invariant);

    await _manager.SaveAsync(content, _authorType, _cancellationToken);

    _contentRepository.Verify(x => x.SaveAsync(content, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should save published contents when the field values are valid.")]
  public async Task Given_PublishedValidFieldValues_When_SaveAsync_Then_Saved()
  {
    FieldTypeId[] fieldTypeIds = [_emailAddressType.Id, _personNameType.Id];
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(fieldTypeIds)), _cancellationToken))
      .ReturnsAsync([_personNameType]);

    _fieldValueValidatorFactory.Setup(x => x.Create(_personNameType)).Returns(new StringValueValidator((StringProperties)_personNameType.Properties));

    ContentLocale invariant = new(
      new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName),
      new DisplayName(_faker.Person.FullName),
      description: null,
      fieldValues: new Dictionary<Guid, string>
      {
        [_firstName.Id] = _faker.Person.FirstName,
        [_lastName.Id] = _faker.Person.LastName
      });
    Content content = new(_authorType, invariant);
    content.PublishInvariant();

    await _manager.SaveAsync(content, _authorType, _cancellationToken);

    _contentRepository.Verify(x => x.SaveAsync(content, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should save contents when there is no unique name conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved(bool found)
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<FieldTypeId>>(), _cancellationToken)).ReturnsAsync([]);

    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName));
    Content content = new(_authorType, invariant);
    if (found)
    {
      _contentQuerier.Setup(x => x.FindIdAsync(_authorType.Id, null, invariant.UniqueName, _cancellationToken)).ReturnsAsync(content.Id);
    }

    await _manager.SaveAsync(content, _authorType, _cancellationToken);

    _contentQuerier.Verify(x => x.FindIdAsync(_authorType.Id, null, invariant.UniqueName, _cancellationToken), Times.Once);
    _contentRepository.Verify(x => x.SaveAsync(content, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the content is not of the specified type.")]
  public async Task Given_InvalidContentType_When_SaveAsync_Then_ArgumentException()
  {
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName));
    Content content = new(_authorType, invariant);

    var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _manager.SaveAsync(content, _articleType, _cancellationToken));
    Assert.StartsWith($"The content type 'Id={_articleType.Id}' was not expected. The expected content type for content 'Id={content.Id}' is '{content.ContentTypeId}'.", exception.Message);
    Assert.Equal("contentType", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ContentFieldValueConflictException when unique field values are already used.")]
  public async Task Given_UniqueValueConflicts_When_SaveAsync_Then_ContentFieldValueConflictException()
  {
    FieldTypeId[] fieldTypeIds = [_emailAddressType.Id, _personNameType.Id];
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(fieldTypeIds)), _cancellationToken))
      .ReturnsAsync([_emailAddressType, _personNameType]);

    _fieldValueValidatorFactory.Setup(x => x.Create(_emailAddressType)).Returns(new StringValueValidator((StringProperties)_emailAddressType.Properties));

    ContentLocale invariant = new(
      new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName),
      new DisplayName(_faker.Person.FullName),
      fieldValues: new Dictionary<Guid, string>
      {
        [_emailAddress.Id] = _faker.Person.Email
      });
    Content content = new(_authorType, invariant);

    ContentId conflictId = ContentId.NewId(realmId: null);
    _contentQuerier.Setup(x => x.FindConflictsAsync(
      content.ContentTypeId,
      null,
      It.Is<IReadOnlyDictionary<Guid, string>>(y => y.Count == 1 && y.Single().Key == _emailAddress.Id && y.Single().Value == _faker.Person.Email),
      content.Id,
      _cancellationToken)).ReturnsAsync(new Dictionary<Guid, ContentId> { [_emailAddress.Id] = conflictId });

    var exception = await Assert.ThrowsAsync<ContentFieldValueConflictException>(async () => await _manager.SaveAsync(content, _authorType, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ContentUniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_ContentUniqueNameAlreadyUsedException()
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<FieldTypeId>>(), _cancellationToken)).ReturnsAsync([]);

    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName));
    Content content = new(_authorType, invariant);

    Content conflict = new(_authorType, invariant);
    _contentQuerier.Setup(x => x.FindIdAsync(_authorType.Id, null, invariant.UniqueName, _cancellationToken)).ReturnsAsync(conflict.Id);

    var exception = await Assert.ThrowsAsync<ContentUniqueNameAlreadyUsedException>(async () => await _manager.SaveAsync(content, _authorType, _cancellationToken));
    Assert.Equal(content.ContentTypeId.EntityId, exception.ContentTypeId);
    Assert.Null(exception.LanguageId);
    Assert.Equal(conflict.EntityId, exception.ConflictId);
    Assert.Equal(content.EntityId, exception.EntityId);
    Assert.Equal(invariant.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when contents contain invalid field values.")]
  public async Task Given_InvalidFields_When_SaveAsync_Then_ValidationException()
  {
    FieldTypeId[] fieldTypeIds = [_emailAddressType.Id, _personNameType.Id];
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(fieldTypeIds)), _cancellationToken))
      .ReturnsAsync([_emailAddressType, _personNameType]);

    _fieldValueValidatorFactory.Setup(x => x.Create(_personNameType)).Returns(new StringValueValidator((StringProperties)_personNameType.Properties));

    string firstName = RandomStringGenerator.GetString(1);
    string lastName = RandomStringGenerator.GetString(999);
    ContentLocale invariant = new(
      new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName),
      new DisplayName(_faker.Person.FullName),
      fieldValues: new Dictionary<Guid, string>
      {
        [_firstName.Id] = firstName,
        [_lastName.Id] = lastName
      });
    Content content = new(_authorType, invariant);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _manager.SaveAsync(content, _authorType, _cancellationToken));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MinimumLengthValidator" && e.ErrorMessage == "The length of the value must be at least 2 characters."
      && e.AttemptedValue.Equals(firstName) && e.PropertyName == $"FieldValues.{_firstName.Id}");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.ErrorMessage == "The length of the value may not exceed 100 characters."
      && e.AttemptedValue.Equals(lastName) && e.PropertyName == $"FieldValues.{_lastName.Id}");
  }

  [Fact(DisplayName = "It should throw ValidationException when contents contain unexpected fields.")]
  public async Task Given_UnexpectedFields_When_SaveAsync_Then_ValidationException()
  {
    FieldTypeId[] fieldTypeIds = [_emailAddressType.Id, _personNameType.Id];
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(fieldTypeIds)), _cancellationToken))
      .ReturnsAsync([_emailAddressType, _personNameType]);

    _fieldValueValidatorFactory.Setup(x => x.Create(_personNameType)).Returns(new StringValueValidator((StringProperties)_personNameType.Properties));

    Guid birthdateId = Guid.Empty;
    Guid genderId = Guid.NewGuid();
    ContentLocale invariant = new(
      new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName),
      new DisplayName(_faker.Person.FullName),
      fieldValues: new Dictionary<Guid, string>
      {
        [_firstName.Id] = _faker.Person.FirstName,
        [_lastName.Id] = _faker.Person.LastName,
        [birthdateId] = _faker.Person.DateOfBirth.ToISOString(),
        [genderId] = _faker.Person.Gender.ToString().ToLowerInvariant()
      });
    Content content = new(_authorType, invariant);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _manager.SaveAsync(content, _authorType, _cancellationToken));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UnexpectedFieldValidator" && e.ErrorMessage == "The specified field was not expected."
      && e.AttemptedValue.Equals(birthdateId) && e.PropertyName == "FieldValues");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UnexpectedFieldValidator" && e.ErrorMessage == "The specified field was not expected."
      && e.AttemptedValue.Equals(genderId) && e.PropertyName == "FieldValues");
  }

  [Fact(DisplayName = "It should throw ValidationException when published contents are missing fields.")]
  public async Task Given_MissingFields_When_SaveAsync_Then_ValidationException()
  {
    FieldTypeId[] fieldTypeIds = [_emailAddressType.Id, _personNameType.Id];
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.SequenceEqual(fieldTypeIds)), _cancellationToken))
      .ReturnsAsync([_personNameType]);

    _fieldValueValidatorFactory.Setup(x => x.Create(_personNameType)).Returns(new StringValueValidator((StringProperties)_personNameType.Properties));

    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName), new DisplayName(_faker.Person.FullName));
    Content content = new(_authorType, invariant);
    content.PublishInvariant();

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _manager.SaveAsync(content, _authorType, _cancellationToken));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RequiredFieldValidator" && e.ErrorMessage == "The specified field is missing."
      && e.AttemptedValue.Equals(_firstName.Id) && e.PropertyName == "FieldValues");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RequiredFieldValidator" && e.ErrorMessage == "The specified field is missing."
      && e.AttemptedValue.Equals(_lastName.Id) && e.PropertyName == "FieldValues");
  }
}
