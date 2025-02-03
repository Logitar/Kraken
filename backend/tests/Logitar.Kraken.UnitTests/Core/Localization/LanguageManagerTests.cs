using Moq;

namespace Logitar.Kraken.Core.Localization;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageManagerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly LanguageManager _manager;

  public LanguageManagerTests()
  {
    _manager = new(_languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "It should save the language.")]
  public async Task Given_Language_When_SaveAsync_Then_LanguageSaved()
  {
    Language language = new(new Locale("fr"));
    language.ClearChanges();

    language.SetDefault();
    await _manager.SaveAsync(language, _cancellationToken);

    _languageQuerier.Verify(x => x.FindIdAsync(It.IsAny<Locale>(), It.IsAny<CancellationToken>()), Times.Never);
    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should save the language when no conflict occurs.")]
  public async Task Given_NoConflict_When_SaveAsync_Then_LanguageSaved()
  {
    Language language = new(new Locale("fr"));

    _languageQuerier.Setup(x => x.FindIdAsync(language.Locale, _cancellationToken)).ReturnsAsync(language.Id);

    await _manager.SaveAsync(language, _cancellationToken);

    _languageQuerier.Verify(x => x.FindIdAsync(language.Locale, _cancellationToken), Times.Once);
    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw LocaleAlreadyUsedException when a language conflict occurs.")]
  public async Task Given_LocaleConflict_When_SaveAsync_Then_LocaleAlreadyUsedException()
  {
    Language language = new(new Locale("fr"));

    LanguageId conflictId = LanguageId.NewId(realmId: null);
    _languageQuerier.Setup(x => x.FindIdAsync(language.Locale, _cancellationToken)).ReturnsAsync(conflictId);

    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await _manager.SaveAsync(language, _cancellationToken));
    Assert.Equal(conflictId.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(conflictId.EntityId, exception.ConflictId);
    Assert.Equal(language.Id.EntityId, exception.LanguageId);
    Assert.Equal(language.Locale.ToString(), exception.Locale);
    Assert.Equal("Locale", exception.PropertyName);

    _languageRepository.Verify(x => x.SaveAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
  }
}
