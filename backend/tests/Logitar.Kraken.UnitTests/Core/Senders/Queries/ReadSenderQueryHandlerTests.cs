using Logitar.Kraken.Contracts.Senders;
using Moq;

namespace Logitar.Kraken.Core.Senders.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadSenderQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISenderQuerier> _senderQuerier = new();

  private readonly ReadSenderQueryHandler _handler;

  private readonly SenderModel _email = new()
  {
    Id = Guid.NewGuid(),
    IsDefault = true,
    Type = SenderType.Email
  };
  private readonly SenderModel _phone = new()
  {
    Id = Guid.NewGuid(),
    IsDefault = true,
    Type = SenderType.Phone
  };

  public ReadSenderQueryHandlerTests()
  {
    _handler = new(_senderQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when no sender was found.")]
  public async Task Given_NoSenderFound_When_Handle_Then_NullReturned()
  {
    ReadSenderQuery query = new(_email.Id, SenderType.Email);
    SenderModel? sender = await _handler.Handle(query, _cancellationToken);
    Assert.Null(sender);
  }

  [Fact(DisplayName = "It should return the sender found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_SenderReturned()
  {
    _senderQuerier.Setup(x => x.ReadAsync(_email.Id, _cancellationToken)).ReturnsAsync(_email);

    ReadSenderQuery query = new(_email.Id, SenderType.Email);
    SenderModel? sender = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(sender);
    Assert.Same(_email, sender);
  }

  [Fact(DisplayName = "It should return the sender found by unique name.")]
  public async Task Given_FoundByUniqueKey_When_Handle_Then_SenderReturned()
  {
    _senderQuerier.Setup(x => x.ReadDefaultAsync(SenderType.Phone, _cancellationToken)).ReturnsAsync(_phone);

    ReadSenderQuery query = new(Id: null, SenderType.Phone);
    SenderModel? sender = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(sender);
    Assert.Same(_phone, sender);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many senders were found.")]
  public async Task Given_ManySendersFound_When_Handle_Then_TooManyResultsException()
  {
    _senderQuerier.Setup(x => x.ReadAsync(_phone.Id, _cancellationToken)).ReturnsAsync(_email);
    _senderQuerier.Setup(x => x.ReadDefaultAsync(SenderType.Email, _cancellationToken)).ReturnsAsync(_phone);

    ReadSenderQuery query = new(_phone.Id, SenderType.Email);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<SenderModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
