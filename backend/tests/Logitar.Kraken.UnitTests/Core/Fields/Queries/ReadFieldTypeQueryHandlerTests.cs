using Logitar.Kraken.Contracts.Fields;
using Moq;

namespace Logitar.Kraken.Core.Fields.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadFieldTypeQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();

  private readonly ReadFieldTypeQueryHandler _handler;

  private readonly FieldTypeModel _title = new()
  {
    Id = Guid.NewGuid(),
    UniqueName = "ArticleTitle"
  };
  private readonly FieldTypeModel _content = new()
  {
    Id = Guid.NewGuid(),
    UniqueName = "ArticleContent"
  };

  public ReadFieldTypeQueryHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when field type was found.")]
  public async Task Given_NoFieldTypeFound_When_Handle_Then_NullReturned()
  {
    ReadFieldTypeQuery query = new(_title.Id, _content.UniqueName);
    FieldTypeModel? fieldType = await _handler.Handle(query, _cancellationToken);
    Assert.Null(fieldType);
  }

  [Fact(DisplayName = "It should return the field type found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_FieldTypeReturned()
  {
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_title.Id, _cancellationToken)).ReturnsAsync(_title);

    ReadFieldTypeQuery query = new(_title.Id, UniqueName: null);
    FieldTypeModel? fieldType = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(fieldType);
    Assert.Same(_title, fieldType);
  }

  [Fact(DisplayName = "It should return the field type found by unique name.")]
  public async Task Given_FoundByUniqueName_When_Handle_Then_FieldTypeReturned()
  {
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_content.UniqueName, _cancellationToken)).ReturnsAsync(_content);

    ReadFieldTypeQuery query = new(Id: null, _content.UniqueName);
    FieldTypeModel? fieldType = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(fieldType);
    Assert.Same(_content, fieldType);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many field types were found.")]
  public async Task Given_ManyFieldTypesFound_When_Handle_Then_TooManyResultsException()
  {
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_title.Id, _cancellationToken)).ReturnsAsync(_title);
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_content.UniqueName, _cancellationToken)).ReturnsAsync(_content);

    ReadFieldTypeQuery query = new(_title.Id, _content.UniqueName);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<FieldTypeModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
