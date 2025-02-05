using Logitar.Kraken.Core.Realms;
using Moq;

namespace Logitar.Kraken.Core.Localization;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageManagerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly LanguageManager _manager;

  public LanguageManagerTests()
  {
    _manager = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "FindAsync: it should return the language found by ID.")]
  public async Task Given_FoundById_When_FindAsync_Then_LanguageReturned()
  {
    Language language = new(new Locale("fr"));
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    Language result = await _manager.FindAsync($"  {language.EntityId}  ", _cancellationToken);
    Assert.Same(language, result);

    _languageRepository.Verify(x => x.LoadAsync(language.Id, _cancellationToken), Times.Once());
    _languageRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Locale>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "FindAsync: it should return the language found by locale code.")]
  public async Task Given_FoundByLocaleCode_When_FindAsync_Then_LanguageReturned()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Language language = new(new Locale("en"), isDefault: true, actorId: null, LanguageId.NewId(realmId));
    _languageRepository.Setup(x => x.LoadAsync(realmId, language.Locale, _cancellationToken)).ReturnsAsync(language);

    Language result = await _manager.FindAsync($"  {language.Locale.Code}  ", _cancellationToken);
    Assert.Same(language, result);

    _languageRepository.Verify(x => x.LoadAsync(It.IsAny<LanguageId>(), It.IsAny<CancellationToken>()), Times.Never());
    _languageRepository.Verify(x => x.LoadAsync(realmId, language.Locale, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "FindAsync: it should throw LanguageNotFoundException when the language could not be found.")]
  public async Task Given_NotFound_When_FindAsync_Then_LanguageNotFoundException()
  {
    string language = "invalid";
    var exception = await Assert.ThrowsAsync<LanguageNotFoundException>(async () => await _manager.FindAsync(language, _cancellationToken));
    Assert.Null(exception.RealmId);
    Assert.Equal(language, exception.Language);
    Assert.Equal("Language", exception.PropertyName);

    _languageRepository.Verify(x => x.LoadAsync(It.IsAny<LanguageId>(), It.IsAny<CancellationToken>()), Times.Never());
    _languageRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<Locale>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "SaveAsync: it should save the language.")]
  public async Task Given_Language_When_SaveAsync_Then_LanguageSaved()
  {
    Language language = new(new Locale("fr"));
    language.ClearChanges();

    language.SetDefault();
    await _manager.SaveAsync(language, _cancellationToken);

    _languageQuerier.Verify(x => x.FindIdAsync(It.IsAny<Locale>(), It.IsAny<CancellationToken>()), Times.Never);
    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the language when no conflict occurs.")]
  public async Task Given_NoConflict_When_SaveAsync_Then_LanguageSaved()
  {
    Language language = new(new Locale("fr"));

    _languageQuerier.Setup(x => x.FindIdAsync(language.Locale, _cancellationToken)).ReturnsAsync(language.Id);

    await _manager.SaveAsync(language, _cancellationToken);

    _languageQuerier.Verify(x => x.FindIdAsync(language.Locale, _cancellationToken), Times.Once);
    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should throw LocaleAlreadyUsedException when a language conflict occurs.")]
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
