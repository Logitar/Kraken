using Logitar.Kraken.Contracts.Passwords;
using Moq;

namespace Logitar.Kraken.Core.Passwords.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadOneTimePasswordQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IOneTimePasswordQuerier> _oneTimePasswordQuerier = new();

  private readonly ReadOneTimePasswordQueryHandler _handler;

  public ReadOneTimePasswordQueryHandlerTests()
  {
    _handler = new(_oneTimePasswordQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password (OTP) could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    Assert.Null(await _handler.Handle(new ReadOneTimePasswordQuery(Guid.NewGuid()), _cancellationToken));
  }

  [Fact(DisplayName = "It should return the One-Time Password (OTP) found by ID.")]
  public async Task Given_Found_When_Handle_Then_OneTimePasswordReturned()
  {
    OneTimePasswordModel oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    _oneTimePasswordQuerier.Setup(x => x.ReadAsync(oneTimePassword.Id, _cancellationToken)).ReturnsAsync(oneTimePassword);

    ReadOneTimePasswordQuery query = new(oneTimePassword.Id);
    OneTimePasswordModel? result = await _handler.Handle(query, _cancellationToken);

    Assert.NotNull(result);
    Assert.Same(oneTimePassword, result);
  }
}
