using Logitar.Kraken.Contracts.Templates;
using Moq;

namespace Logitar.Kraken.Core.Templates.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadTemplateQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ITemplateQuerier> _templateQuerier = new();

  private readonly ReadTemplateQueryHandler _handler;

  private readonly TemplateModel _accountActivation = new()
  {
    Id = Guid.NewGuid(),
    UniqueKey = "AccountActivation"
  };
  private readonly TemplateModel _passwordRecovery = new()
  {
    Id = Guid.NewGuid(),
    UniqueKey = "PasswordRecovery"
  };

  public ReadTemplateQueryHandlerTests()
  {
    _handler = new(_templateQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when no template was found.")]
  public async Task Given_NoTemplateFound_When_Handle_Then_NullReturned()
  {
    ReadTemplateQuery query = new(_accountActivation.Id, _passwordRecovery.UniqueKey);
    TemplateModel? template = await _handler.Handle(query, _cancellationToken);
    Assert.Null(template);
  }

  [Fact(DisplayName = "It should return the template found by ID.")]
  public async Task Given_FoundById_When_Handle_Then_TemplateReturned()
  {
    _templateQuerier.Setup(x => x.ReadAsync(_accountActivation.Id, _cancellationToken)).ReturnsAsync(_accountActivation);

    ReadTemplateQuery query = new(_accountActivation.Id, UniqueKey: null);
    TemplateModel? template = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(template);
    Assert.Same(_accountActivation, template);
  }

  [Fact(DisplayName = "It should return the template found by unique name.")]
  public async Task Given_FoundByUniqueKey_When_Handle_Then_TemplateReturned()
  {
    _templateQuerier.Setup(x => x.ReadAsync(_passwordRecovery.UniqueKey, _cancellationToken)).ReturnsAsync(_passwordRecovery);

    ReadTemplateQuery query = new(Id: null, _passwordRecovery.UniqueKey);
    TemplateModel? template = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(template);
    Assert.Same(_passwordRecovery, template);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many templates were found.")]
  public async Task Given_ManyTemplatesFound_When_Handle_Then_TooManyResultsException()
  {
    _templateQuerier.Setup(x => x.ReadAsync(_accountActivation.Id, _cancellationToken)).ReturnsAsync(_accountActivation);
    _templateQuerier.Setup(x => x.ReadAsync(_passwordRecovery.UniqueKey, _cancellationToken)).ReturnsAsync(_passwordRecovery);

    ReadTemplateQuery query = new(_accountActivation.Id, _passwordRecovery.UniqueKey);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<TemplateModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
