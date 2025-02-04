using Bogus;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Senders.Settings;
using Logitar.Kraken.Core.Users;
using Moq;

namespace Logitar.Kraken.Core.Senders.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteSenderCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ISenderQuerier> _senderQuerier = new();
  private readonly Mock<ISenderRepository> _senderRepository = new();

  private readonly DeleteSenderCommandHandler _handler;

  private readonly Sender _sender;

  public DeleteSenderCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _senderQuerier.Object, _senderRepository.Object);

    Email email = new(_faker.Person.Email);
    SendGridSettings settings = new("SG.API.key");
    _sender = new(email, phone: null, settings, actorId: null, SenderId.NewId(_realmId));
    _sender.SetDefault();
    _senderRepository.Setup(x => x.LoadAsync(_sender.Id, _cancellationToken)).ReturnsAsync(_sender);
  }

  [Fact(DisplayName = "It should delete an existing sender.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    _senderRepository.Setup(x => x.LoadAsync(_realmId, _cancellationToken)).ReturnsAsync([_sender]);

    SenderModel model = new();
    _senderQuerier.Setup(x => x.ReadAsync(_sender, _cancellationToken)).ReturnsAsync(model);

    DeleteSenderCommand command = new(_sender.EntityId);
    SenderModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(_sender.IsDeleted);

    _senderRepository.Verify(x => x.LoadAsync(_realmId, _cancellationToken), Times.Once());
    _senderRepository.Verify(x => x.SaveAsync(_sender, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should delete the default sender when there is no other senders of the same type in the realm.")]
  public async Task Given_NoOtherSenders_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    Sender phone = new(email: null, new Phone("+15148454636"), new TwilioSettings("AccountSid", "AuthenticationToken"), actorId: null, SenderId.NewId(_realmId));
    _senderRepository.Setup(x => x.LoadAsync(_realmId, _cancellationToken)).ReturnsAsync([_sender, phone]);

    SenderModel model = new();
    _senderQuerier.Setup(x => x.ReadAsync(_sender, _cancellationToken)).ReturnsAsync(model);

    DeleteSenderCommand command = new(_sender.EntityId);
    SenderModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(_sender.IsDeleted);

    _senderRepository.Verify(x => x.LoadAsync(_realmId, _cancellationToken), Times.Once());
    _senderRepository.Verify(x => x.SaveAsync(_sender, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the sender could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteSenderCommand command = new(_sender.EntityId);
    SenderModel? sender = await _handler.Handle(command, _cancellationToken);
    Assert.Null(sender);
  }

  [Fact(DisplayName = "It should throw CannotDeleteDefaultSenderException when there are other senders of the same type in the realm.")]
  public async Task Given_OtherSenders_When_Handle_Then_CannotDeleteDefaultSenderException()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    Sender sender = new(_sender.Email, _sender.Phone, _sender.Settings, actorId: null, SenderId.NewId(_realmId));
    _senderRepository.Setup(x => x.LoadAsync(sender.Id, _cancellationToken)).ReturnsAsync(sender);

    Sender phone = new(email: null, new Phone("+15148454636"), new TwilioSettings("AccountSid", "AuthenticationToken"), actorId: null, SenderId.NewId(_realmId));
    _senderRepository.Setup(x => x.LoadAsync(_realmId, _cancellationToken)).ReturnsAsync([_sender, sender, phone]);

    DeleteSenderCommand command = new(_sender.EntityId);
    var exception = await Assert.ThrowsAsync<CannotDeleteDefaultSenderException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_realmId.ToGuid(), exception.RealmId);
    Assert.Equal(_sender.EntityId, exception.SenderId);
    Assert.Equal(_sender.Type, exception.SenderType);
  }
}
