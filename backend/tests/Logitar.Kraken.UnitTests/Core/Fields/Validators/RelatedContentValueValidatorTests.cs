using Bogus;
using FluentValidation.Results;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Properties;
using Moq;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Logitar.Kraken.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class RelatedContentValueValidatorTests
{
  private const string PropertyName = "Author";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IContentQuerier> _contentQuerier = new();

  private readonly ContentType _articleType = new(new Identifier("BlogArticle"), isInvariant: false);
  private readonly ContentType _authorType = new(new Identifier("BlogAuthor"));
  private readonly ContentType _categoryType = new(new Identifier("BlogCategory"));

  private readonly Content _article;
  private readonly Content _author1;
  private readonly Content _author2;
  private readonly Content _category;

  private readonly RelatedContentProperties _properties;
  private readonly RelatedContentValueValidator _validator;

  public RelatedContentValueValidatorTests()
  {
    _article = new(_articleType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "my-blog-article"), new DisplayName("My Blog Article!")));
    _author1 = new(_authorType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName), new DisplayName(_faker.Person.FullName)));
    _author2 = new(_authorType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, _faker.Internet.UserName()), new DisplayName(_faker.Name.FullName())));
    _category = new(_categoryType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "software-architecture")));

    _properties = new(_authorType.Id, isMultiple: false);
    _validator = new(_contentQuerier.Object, _properties);
  }

  [Fact(DisplayName = "Validation should fail when contents are from another content type.")]
  public async Task Given_OtherContentType_When_ValidateAsync_Then_FailureResult()
  {
    RelatedContentProperties properties = new(_properties.ContentTypeId, isMultiple: true);
    RelatedContentValueValidator validator = new(_contentQuerier.Object, properties);

    _contentQuerier.Setup(x => x.FindContentTypeIdsAsync(
      It.Is<IEnumerable<Guid>>(y => y.SequenceEqual(new Guid[] { _article.EntityId, _author1.EntityId, _author2.EntityId, _category.EntityId })),
      _cancellationToken)).ReturnsAsync(new Dictionary<Guid, Guid>
      {
        [_article.EntityId] = _article.ContentTypeId.EntityId,
        [_author1.EntityId] = _author1.ContentTypeId.EntityId,
        [_author2.EntityId] = _author2.ContentTypeId.EntityId,
        [_category.EntityId] = _category.ContentTypeId.EntityId
      });

    string value = $@"[ ""{_article.EntityId}"", ""{_author1.EntityId}"", ""{_author2.EntityId}"", ""{_category.EntityId}"" ]";
    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.ErrorMessage == $"The content type 'Id={_articleType.EntityId}' does not match the expected content type 'Id={_authorType.EntityId}'."
      && e.AttemptedValue.Equals(_article.EntityId) && e.PropertyName == PropertyName
      && HasProperty(e.CustomState, "ExpectedContentTypeId", _authorType.EntityId)
      && HasProperty(e.CustomState, "ActualContentTypeId", _articleType.EntityId));
    Assert.Contains(result.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.ErrorMessage == $"The content type 'Id={_categoryType.EntityId}' does not match the expected content type 'Id={_authorType.EntityId}'."
      && e.AttemptedValue.Equals(_category.EntityId) && e.PropertyName == PropertyName
      && HasProperty(e.CustomState, "ExpectedContentTypeId", _authorType.EntityId)
      && HasProperty(e.CustomState, "ActualContentTypeId", _categoryType.EntityId));
  }

  [Fact(DisplayName = "Validation should fail when contents could not be found.")]
  public async Task Given_NotFound_When_ValidateAsync_Then_FailureResult()
  {
    RelatedContentProperties properties = new(_properties.ContentTypeId, isMultiple: true);
    RelatedContentValueValidator validator = new(_contentQuerier.Object, properties);

    Guid missing1 = Guid.NewGuid();
    Guid missing2 = Guid.NewGuid();
    _contentQuerier.Setup(x => x.FindContentTypeIdsAsync(
      It.Is<IEnumerable<Guid>>(y => y.SequenceEqual(new Guid[] { _author1.EntityId, missing1, missing2 })),
      _cancellationToken)).ReturnsAsync(new Dictionary<Guid, Guid> { [_author1.EntityId] = _author1.ContentTypeId.EntityId });

    string value = $@"[ ""{_author1.EntityId}"", ""{missing1}"", ""{missing2}"" ]";
    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "ContentValidator" && e.ErrorMessage == "The content could not be found."
      && e.AttemptedValue.Equals(missing1) && e.PropertyName == PropertyName);
    Assert.Contains(result.Errors, e => e.ErrorCode == "ContentValidator" && e.ErrorMessage == "The content could not be found."
      && e.AttemptedValue.Equals(missing2) && e.PropertyName == PropertyName);
  }

  [Theory(DisplayName = "Validation should fail when the value is not valid.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData("invalid")]
  [InlineData(@"[""7a9aed82-f486-4975-8078-bd562c045aa1""]")]
  public async Task Given_InvalidValue_When_ValidateAsync_Then_FailureResult(string value)
  {
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);

    ValidationFailure failure = Assert.Single(result.Errors);
    Assert.Equal(value, failure.AttemptedValue);
    Assert.Equal("RelatedContentValueValidator", failure.ErrorCode);
    Assert.Equal("The value must be a valid content ID.", failure.ErrorMessage);
    Assert.Equal(PropertyName, failure.PropertyName);
  }

  [Theory(DisplayName = "Validation should fail when the values are not valid.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData("ca8289b5-68b9-4708-ba1a-65503202b911")]
  [InlineData(@"[""invalid""]")]
  public async Task Given_InvalidValues_When_ValidateAsync_Then_FailureResult(string value)
  {
    RelatedContentProperties properties = new(_properties.ContentTypeId, isMultiple: true);
    RelatedContentValueValidator validator = new(_contentQuerier.Object, properties);

    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);

    ValidationFailure failure = Assert.Single(result.Errors);
    Assert.Equal(value, failure.AttemptedValue);
    Assert.Equal("RelatedContentValueValidator", failure.ErrorCode);
    Assert.Equal("The value must be a JSON-serialized content ID array.", failure.ErrorMessage);
    Assert.Equal(PropertyName, failure.PropertyName);
  }

  [Fact(DisplayName = "Validation should succeed when the value is empty.")]
  public async Task Given_Empty_When_ValidateAsync_Then_FailureResult()
  {
    RelatedContentProperties properties = new(_properties.ContentTypeId, isMultiple: true);
    RelatedContentValueValidator validator = new(_contentQuerier.Object, properties);

    string value = " [  ] ";
    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }

  [Theory(DisplayName = "Validation should succeed when the value is valid.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult(bool isMultiple)
  {
    string value = _author1.EntityId.ToString();

    RelatedContentValueValidator validator = _validator;
    if (isMultiple)
    {
      RelatedContentProperties properties = new(_properties.ContentTypeId, isMultiple: true);
      validator = new(_contentQuerier.Object, properties);

      value = $@"[ ""{_author1.EntityId}"", ""{_author2.EntityId}"" ]";
    }

    _contentQuerier.Setup(x => x.FindContentTypeIdsAsync(
      It.Is<IEnumerable<Guid>>(y => y.SequenceEqual(isMultiple ? new Guid[] { _author1.EntityId, _author2.EntityId } : new Guid[] { _author1.EntityId })),
      _cancellationToken)).ReturnsAsync(new Dictionary<Guid, Guid>
      {
        [_author1.EntityId] = _author1.ContentTypeId.EntityId,
        [_author2.EntityId] = _author2.ContentTypeId.EntityId
      });

    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }

  private static bool HasProperty(object instance, string propertyName, object? propertyValue)
  {
    PropertyInfo? property = instance.GetType().GetProperty(propertyName);
    Assert.NotNull(property);

    object? value = property.GetValue(instance);
    return propertyValue == null ? value == null : propertyValue.Equals(value);
  }
}
