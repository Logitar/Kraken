using Bogus;
using Logitar.Kraken.Contracts.Contents;
using Moq;

namespace Logitar.Kraken.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadPublishedContentQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IPublishedContentQuerier> _publishedContentQuerier = new();

  private readonly ReadPublishedContentQueryHandler _handler;

  private readonly PublishedContent _article = new()
  {
    Id = Guid.NewGuid()
  };
  private readonly PublishedContent _author = new()
  {
    Id = Guid.NewGuid()
  };
  private readonly PublishedContent _category = new()
  {
    Id = Guid.NewGuid()
  };

  public ReadPublishedContentQueryHandlerTests()
  {
    _handler = new(_publishedContentQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when content was found.")]
  public async Task Given_NoContentTypeFound_When_Handle_Then_NullReturned()
  {
    PublishedContentKey key = new(Guid.NewGuid().ToString(), Language: null, _faker.Person.UserName);
    ReadPublishedContentQuery query = new(ContentId: null, _article.Id, key);
    PublishedContent? content = await _handler.Handle(query, _cancellationToken);
    Assert.Null(content);
  }

  [Fact(DisplayName = "It should return the content found by Guid.")]
  public async Task Given_FoundByGuid_When_Handle_Then_ContentReturned()
  {
    _publishedContentQuerier.Setup(x => x.ReadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);

    ReadPublishedContentQuery query = new(ContentId: null, _article.Id, Key: null);
    PublishedContent? content = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(content);
    Assert.Same(_article, content);
  }

  [Fact(DisplayName = "It should return the content found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_ContentReturned()
  {
    int contentId = 123;
    _publishedContentQuerier.Setup(x => x.ReadAsync(contentId, _cancellationToken)).ReturnsAsync(_category);

    ReadPublishedContentQuery query = new(contentId, ContentUid: null, Key: null);
    PublishedContent? content = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(content);
    Assert.Same(_category, content);
  }

  [Fact(DisplayName = "It should return the content found by unique name.")]
  public async Task Given_FoundByUniqueName_When_Handle_Then_ContentReturned()
  {
    PublishedContentKey key = new(Guid.NewGuid().ToString(), Language: null, _faker.Person.UserName);
    _publishedContentQuerier.Setup(x => x.ReadAsync(key, _cancellationToken)).ReturnsAsync(_author);

    ReadPublishedContentQuery query = new(ContentId: null, ContentUid: null, key);
    PublishedContent? content = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(content);
    Assert.Same(_author, content);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many content types were found.")]
  public async Task Given_ManyContentTypesFound_When_Handle_Then_TooManyResultsException()
  {
    int contentId = 987;
    PublishedContentKey key = new(Guid.NewGuid().ToString(), Language: null, _faker.Person.UserName);
    _publishedContentQuerier.Setup(x => x.ReadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);
    _publishedContentQuerier.Setup(x => x.ReadAsync(key, _cancellationToken)).ReturnsAsync(_author);
    _publishedContentQuerier.Setup(x => x.ReadAsync(contentId, _cancellationToken)).ReturnsAsync(_category);

    ReadPublishedContentQuery query = new(contentId, _article.Id, key);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<PublishedContent>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }
}
