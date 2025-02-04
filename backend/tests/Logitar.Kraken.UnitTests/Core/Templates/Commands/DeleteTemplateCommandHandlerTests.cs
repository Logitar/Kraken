using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.Templates.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteTemplateCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ITemplateQuerier> _templateQuerier = new();
  private readonly Mock<ITemplateRepository> _templateRepository = new();

  private readonly DeleteTemplateCommandHandler _handler;

  private readonly Template _template;

  public DeleteTemplateCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _templateQuerier.Object, _templateRepository.Object);

    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    TemplateContent content = TemplateContent.PlainText("{Token}");
    _template = new(uniqueKey, subject, content, actorId: null, TemplateId.NewId(_realmId));
    _templateRepository.Setup(x => x.LoadAsync(_template.Id, _cancellationToken)).ReturnsAsync(_template);
  }

  [Fact(DisplayName = "It should delete an existing template.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    TemplateModel model = new();
    _templateQuerier.Setup(x => x.ReadAsync(_template, _cancellationToken)).ReturnsAsync(model);

    DeleteTemplateCommand command = new(_template.EntityId);
    TemplateModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(_template.IsDeleted);

    _templateRepository.Verify(x => x.SaveAsync(_template, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the template could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteTemplateCommand command = new(_template.EntityId);
    TemplateModel? template = await _handler.Handle(command, _cancellationToken);
    Assert.Null(template);
  }
}
