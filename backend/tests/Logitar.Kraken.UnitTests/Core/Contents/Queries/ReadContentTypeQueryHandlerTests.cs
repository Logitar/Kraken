using Logitar.Kraken.Contracts.Contents;
using Moq;

namespace Logitar.Kraken.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadContentTypeQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();

  private readonly ReadContentTypeQueryHandler _handler;

  private readonly ContentTypeModel _article = new()
  {
    Id = Guid.NewGuid(),
    IsInvariant = false,
    UniqueName = "BlogArticle"
  };
  private readonly ContentTypeModel _author = new()
  {
    Id = Guid.NewGuid(),
    IsInvariant = true,
    UniqueName = "BlogAuthor"
  };

  public ReadContentTypeQueryHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when content type was found.")]
  public async Task Given_NoContentTypeFound_When_Handle_Then_NullReturned()
  {
    ReadContentTypeQuery query = new(_article.Id, _author.UniqueName);
    ContentTypeModel? contentType = await _handler.Handle(query, _cancellationToken);
    Assert.Null(contentType);
  }

  [Fact(DisplayName = "It should return the content type found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_ContentTypeReturned()
  {
    _contentTypeQuerier.Setup(x => x.ReadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);

    ReadContentTypeQuery query = new(_article.Id, UniqueName: null);
    ContentTypeModel? contentType = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Same(_article, contentType);
  }

  [Fact(DisplayName = "It should return the content type found by unique name.")]
  public async Task Given_FoundByUniqueName_When_Handle_Then_ContentTypeReturned()
  {
    _contentTypeQuerier.Setup(x => x.ReadAsync(_author.UniqueName, _cancellationToken)).ReturnsAsync(_author);

    ReadContentTypeQuery query = new(Id: null, _author.UniqueName);
    ContentTypeModel? contentType = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Same(_author, contentType);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many content types were found.")]
  public async Task Given_ManyContentTypesFound_When_Handle_Then_TooManyResultsException()
  {
    _contentTypeQuerier.Setup(x => x.ReadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);
    _contentTypeQuerier.Setup(x => x.ReadAsync(_author.UniqueName, _cancellationToken)).ReturnsAsync(_author);

    ReadContentTypeQuery query = new(_article.Id, _author.UniqueName);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<ContentTypeModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
