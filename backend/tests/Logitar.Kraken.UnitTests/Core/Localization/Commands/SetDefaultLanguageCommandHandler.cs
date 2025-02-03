using Logitar.Kraken.Contracts.Localization;
using Moq;

namespace Logitar.Kraken.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetDefaultLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly SetDefaultLanguageCommandHandler _handler;

  private readonly Language _english = new(new Locale("en"), isDefault: true);
  private readonly Language _french = new(new Locale("fr"));

  public SetDefaultLanguageCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object);

    _languageQuerier.Setup(x => x.FindDefaultIdAsync(_cancellationToken)).ReturnsAsync(_english.Id);
    _languageRepository.Setup(x => x.LoadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);
    _languageRepository.Setup(x => x.LoadAsync(_french.Id, _cancellationToken)).ReturnsAsync(_french);
  }

  [Fact(DisplayName = "Handle: it should not do anything when the language is already default.")]
  public async Task Given_LanguageIsDefault_When_Handle_Then_DoNothing()
  {
    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(_english, _cancellationToken)).ReturnsAsync(model);

    SetDefaultLanguageCommand command = new(_english.Id.EntityId);
    LanguageModel? language = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(language);
    Assert.Same(model, language);

    _languageRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Language>>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "Handle: it should return null when the language was not found.")]
  public async Task Given_LanguageNotFound_When_Handle_Then_NullReturned()
  {
    Assert.Null(await _handler.Handle(new SetDefaultLanguageCommand(Guid.NewGuid()), _cancellationToken));
  }

  [Fact(DisplayName = "Handle: it should set the default language.")]
  public async Task Given_LanguageNotDefault_When_Handle_Then_DefaultLanguageSet()
  {
    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(_french, _cancellationToken)).ReturnsAsync(model);

    SetDefaultLanguageCommand command = new(_french.Id.EntityId);
    LanguageModel? language = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(language);
    Assert.Same(model, language);

    _languageRepository.Verify(x => x.SaveAsync(
      It.Is<IEnumerable<Language>>(y => y.SequenceEqual(new Language[] { _english, _french })),
      _cancellationToken), Times.Once);

    Assert.False(_english.IsDefault);
    Assert.True(_french.IsDefault);
  }
}
