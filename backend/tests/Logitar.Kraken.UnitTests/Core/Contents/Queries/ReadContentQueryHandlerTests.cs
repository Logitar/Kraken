using Bogus;
using Logitar.Kraken.Contracts.Contents;
using Moq;

namespace Logitar.Kraken.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadContentQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IContentQuerier> _contentQuerier = new();

  private readonly ReadContentQueryHandler _handler;

  private readonly ContentModel _article = new()
  {
    Id = Guid.NewGuid()
  };
  private readonly ContentModel _author = new()
  {
    Id = Guid.NewGuid()
  };

  public ReadContentQueryHandlerTests()
  {
    _handler = new(_contentQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when content was found.")]
  public async Task Given_NoContentTypeFound_When_Handle_Then_NullReturned()
  {
    ContentKey key = new(Guid.NewGuid(), LanguageId: null, _faker.Person.UserName);
    ReadContentQuery query = new(_article.Id, key);
    ContentModel? content = await _handler.Handle(query, _cancellationToken);
    Assert.Null(content);
  }

  [Fact(DisplayName = "It should return the content found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_ContentReturned()
  {
    _contentQuerier.Setup(x => x.ReadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);

    ReadContentQuery query = new(_article.Id, Key: null);
    ContentModel? content = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(content);
    Assert.Same(_article, content);
  }

  [Fact(DisplayName = "It should return the content found by unique name.")]
  public async Task Given_FoundByUniqueName_When_Handle_Then_ContentReturned()
  {
    ContentKey key = new(Guid.NewGuid(), LanguageId: null, _faker.Person.UserName);
    _contentQuerier.Setup(x => x.ReadAsync(key, _cancellationToken)).ReturnsAsync(_author);

    ReadContentQuery query = new(Id: null, key);
    ContentModel? content = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(content);
    Assert.Same(_author, content);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many content types were found.")]
  public async Task Given_ManyContentTypesFound_When_Handle_Then_TooManyResultsException()
  {
    ContentKey key = new(Guid.NewGuid(), LanguageId: null, _faker.Person.UserName);
    _contentQuerier.Setup(x => x.ReadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);
    _contentQuerier.Setup(x => x.ReadAsync(key, _cancellationToken)).ReturnsAsync(_author);

    ReadContentQuery query = new(_article.Id, key);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<ContentModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
