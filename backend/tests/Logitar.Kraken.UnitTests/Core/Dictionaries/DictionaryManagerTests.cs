using Logitar.Kraken.Core.Localization;
using Moq;

namespace Logitar.Kraken.Core.Dictionaries;

[Trait(Traits.Category, Categories.Unit)]
public class DictionaryManagerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IDictionaryQuerier> _dictionaryQuerier = new();
  private readonly Mock<IDictionaryRepository> _dictionaryRepository = new();

  private readonly DictionaryManager _manager;

  public DictionaryManagerTests()
  {
    _manager = new(_dictionaryQuerier.Object, _dictionaryRepository.Object);
  }

  [Fact(DisplayName = "It should save a dictionary.")]
  public async Task Given_NoChange_When_SaveAsync_Then_Saved()
  {
    Language language = new(new Locale("en"), isDefault: true);
    Dictionary dictionary = new(language);
    dictionary.ClearChanges();

    await _manager.SaveAsync(dictionary, _cancellationToken);

    _dictionaryQuerier.Verify(x => x.FindIdAsync(It.IsAny<LanguageId>(), It.IsAny<CancellationToken>()), Times.Never);
    _dictionaryRepository.Verify(x => x.SaveAsync(dictionary, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should save the dictionary when there is no unique name conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved(bool found)
  {
    Language language = new(new Locale("en"), isDefault: true);
    Dictionary dictionary = new(language);
    if (found)
    {
      _dictionaryQuerier.Setup(x => x.FindIdAsync(dictionary.LanguageId, _cancellationToken)).ReturnsAsync(dictionary.Id);
    }

    await _manager.SaveAsync(dictionary, _cancellationToken);

    _dictionaryQuerier.Verify(x => x.FindIdAsync(dictionary.LanguageId, _cancellationToken), Times.Once);
    _dictionaryRepository.Verify(x => x.SaveAsync(dictionary, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw LanguageAlreadyUsedException when the unique name is already used.")]
  public async Task Given_LanguageConflict_When_SaveAsync_Then_LanguageAlreadyUsedException()
  {
    Language language = new(new Locale("en"), isDefault: true);
    Dictionary dictionary = new(language);

    DictionaryId conflictId = DictionaryId.NewId(realmId: null);
    _dictionaryQuerier.Setup(x => x.FindIdAsync(dictionary.LanguageId, _cancellationToken)).ReturnsAsync(conflictId);

    var exception = await Assert.ThrowsAsync<LanguageAlreadyUsedException>(async () => await _manager.SaveAsync(dictionary, _cancellationToken));
    Assert.Equal(dictionary.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(conflictId.EntityId, exception.ConflictId);
    Assert.Equal(dictionary.EntityId, exception.DictionaryId);
    Assert.Equal(language.EntityId, exception.LanguageId);
    Assert.Equal("LanguageId", exception.PropertyName);
  }
}
