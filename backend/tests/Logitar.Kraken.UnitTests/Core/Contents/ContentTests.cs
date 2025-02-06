using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTests
{
  private readonly Language _english = new(new Locale("en"), isDefault: true);
  private readonly Language _french = new(new Locale("fr"), isDefault: true);
  private readonly ContentType _contentType = new(new Identifier("BlogArticle"), isInvariant: false);
  private readonly Content _content;

  public ContentTests()
  {
    _content = new(_contentType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week")));
  }

  [Fact(DisplayName = "Delete: it should delete the content.")]
  public void Given_Content_When_Delete_Then_Deleted()
  {
    _content.Delete();
    Assert.True(_content.IsDeleted);

    _content.ClearChanges();
    _content.Delete();
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);
  }

  [Fact(DisplayName = "RemoveLocale: it should delete the locale and return true when it is found.")]
  public void Given_LocaleFound_When_RemoveLocale_Then_DeletedAndFalseReturned()
  {
    Language english = new(new Locale("en"), isDefault: true);
    _content.SetLocale(english, _content.Invariant);

    Language french = new(new Locale("fr"), isDefault: true);
    _content.SetLocale(french, _content.Invariant);

    _content.ClearChanges();
    Assert.True(_content.HasLocale(english));
    Assert.True(_content.HasLocale(french));

    ActorId actorId = ActorId.NewId();
    _content.RemoveLocale(english, actorId);
    _content.RemoveLocale(french.Id, actorId);

    Assert.Empty(_content.Locales);
    Assert.Contains(_content.Changes, change => change is ContentLocaleRemoved deleted && deleted.LanguageId == english.Id);
    Assert.Contains(_content.Changes, change => change is ContentLocaleRemoved deleted && deleted.LanguageId == french.Id);
  }

  [Fact(DisplayName = "RemoveLocale: it should return false when the locale could not be found.")]
  public void Given_NoLocale_When_RemoveLocale_Then_FalseReturned()
  {
    Language language = new(new Locale("en"), isDefault: true);

    _content.ClearChanges();
    Assert.False(_content.RemoveLocale(language));
    Assert.False(_content.RemoveLocale(language.Id));
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);
  }

  [Fact(DisplayName = "FindLocale: it should return the locale found.")]
  public void Given_LocaleFound_When_FindLocale_Then_LocaleReturned()
  {
    _content.SetLocale(_english, _content.Invariant);

    ContentLocale locale = _content.FindLocale(_english);
    Assert.Same(_content.Invariant, locale);
  }

  [Fact(DisplayName = "FindLocale: it should throw InvalidOperationException when the locale was not found.")]
  public void Given_NoLocale_When_FindLocale_Then_InvalidOperationException()
  {
    var exception = Assert.Throws<InvalidOperationException>(() => _content.FindLocale(_english));
    Assert.StartsWith($"The locale 'LanguageId={_english.Id}' could not be found.", exception.Message);
  }

  [Fact(DisplayName = "HasLocale: it should return true when the locale was found.")]
  public void Given_LocaleFound_When_HasLocale_Then_TrueReturned()
  {
    _content.SetLocale(_english, _content.Invariant);
    Assert.True(_content.HasLocale(_english));
  }

  [Fact(DisplayName = "HasLocale: it should return false when the locale was not found.")]
  public void Given_NoLocale_When_HasLocale_Then_FalseReturned()
  {
    Assert.False(_content.HasLocale(_english));
  }

  [Fact(DisplayName = "It should not be published initially.")]
  public void Given_NotPublished_When_Created_Then_NotPublished()
  {
    _content.SetLocale(_english, _content.Invariant);

    Assert.False(_content.IsInvariantPublished());
    Assert.False(_content.IsLocalePublished(_english));
  }

  [Fact(DisplayName = "Publish: it should not do anything when the invariant and all locales are published.")]
  public void Given_AllPublished_When_Publish_Then_NoChange()
  {
    _content.PublishInvariant();

    _content.SetLocale(_english, _content.Invariant);
    _content.PublishLocale(_english);

    _content.ClearChanges();
    _content.Publish();
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);
  }

  [Fact(DisplayName = "Publish: it should publish unpublished invariant & locales.")]
  public void Given_NotAllPublished_When_Publish_Then_Published()
  {
    _content.SetLocale(_english, _content.Invariant);

    _content.SetLocale(_french, _content.Invariant);
    _content.PublishLocale(_french);

    _content.ClearChanges();
    _content.Publish();
    Assert.Equal(2, _content.Changes.Count);
    Assert.Contains(_content.Changes, change => change is ContentLocalePublished published && published.LanguageId == null);
    Assert.Contains(_content.Changes, change => change is ContentLocalePublished published && published.LanguageId == _english.Id);
  }

  [Fact(DisplayName = "PublishInvariant: it should not do anything when the invariant is already published.")]
  public void Given_Published_When_PublishInvariant_Then_NoChange()
  {
    _content.PublishInvariant();
    _content.ClearChanges();

    _content.PublishInvariant();
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);

    AssertStatus(language: null, ContentStatus.Latest);
  }

  [Fact(DisplayName = "PublishInvariant: it should publish the invariant when it is not published.")]
  public void Given_NotPublished_When_PublishInvariant_Then_Published()
  {
    _content.PublishInvariant();
    Assert.Contains(_content.Changes, change => change is ContentLocalePublished published && published.LanguageId == null);
    AssertStatus(language: null, ContentStatus.Latest);

    _content.ClearChanges();
    _content.SetInvariant(new ContentLocale(_content.Invariant.UniqueName, new DisplayName("Acura Integra Type S HRC Prototype Debuts at Monterey Car Week")));
    AssertStatus(language: null, ContentStatus.Published);

    _content.PublishInvariant();
    Assert.Contains(_content.Changes, change => change is ContentLocalePublished published && published.LanguageId == null);
    AssertStatus(language: null, ContentStatus.Latest);
  }

  [Fact(DisplayName = "PublishLocale: it should not do anything when the invariant is already published.")]
  public void Given_Published_When_PublishLocale_Then_NoChange()
  {
    _content.SetLocale(_english, _content.Invariant);
    Assert.True(_content.PublishLocale(_english));
    _content.ClearChanges();

    Assert.True(_content.PublishLocale(_english));
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);

    AssertStatus(_english, ContentStatus.Latest);
  }

  [Fact(DisplayName = "PublishLocale: it should publish the locale when it is not published.")]
  public void Given_NotPublished_When_PublishLocale_Then_Published()
  {
    _content.SetLocale(_english, _content.Invariant);
    Assert.True(_content.PublishLocale(_english));
    Assert.Contains(_content.Changes, change => change is ContentLocalePublished published && published.LanguageId == _english.Id);
    AssertStatus(_english, ContentStatus.Latest);

    _content.ClearChanges();
    _content.SetLocale(_english, new ContentLocale(_content.Invariant.UniqueName, new DisplayName("Acura Integra Type S HRC Prototype Debuts at Monterey Car Week")));
    AssertStatus(_english, ContentStatus.Published);

    Assert.True(_content.PublishLocale(_english));
    Assert.Contains(_content.Changes, change => change is ContentLocalePublished published && published.LanguageId == _english.Id);
    AssertStatus(_english, ContentStatus.Latest);
  }

  [Fact(DisplayName = "PublishLocale: it should return false when the locale could not be found.")]
  public void Given_NoLocale_When_PublishLocale_Then_FalseReturned()
  {
    Assert.False(_content.PublishLocale(_english));
  }

  [Fact(DisplayName = "SetInvariant: it should not do anything when the invariant has no change.")]
  public void Given_NoChange_When_SetInvariant_Then_NoChange()
  {
    _content.PublishInvariant();
    _content.ClearChanges();

    _content.SetInvariant(_content.Invariant);
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);

    AssertStatus(language: null, ContentStatus.Latest);
  }

  [Fact(DisplayName = "SetInvariant: it should update the invariant when it has changes.")]
  public void Given_Changes_When_SetInvariant_Then_Changed()
  {
    _content.PublishInvariant();
    _content.ClearChanges();

    _content.SetInvariant(new ContentLocale(_content.Invariant.UniqueName, new DisplayName("Acura Integra Type S HRC Prototype Debuts at Monterey Car Week")));
    Assert.Contains(_content.Changes, change => change is ContentLocaleChanged changed && changed.LanguageId == null);

    AssertStatus(language: null, ContentStatus.Published);
  }

  [Fact(DisplayName = "SetLocale: it should add a new locale.")]
  public void Given_New_When_SetLocale_Then_Added()
  {
    _content.SetLocale(_english, _content.Invariant);
    Assert.Contains(_content.Changes, change => change is ContentLocaleChanged changed && changed.LanguageId == _english.Id);

    AssertStatus(_english, status: null);
  }

  [Fact(DisplayName = "SetLocale: it should not do anything when the locale has no change.")]
  public void Given_NoChange_When_SetLocale_Then_NoChange()
  {
    _content.SetLocale(_english, _content.Invariant);
    _content.PublishLocale(_english);
    _content.ClearChanges();

    _content.SetLocale(_english, _content.Invariant);
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);

    AssertStatus(_english, ContentStatus.Latest);
  }

  [Fact(DisplayName = "SetLocale: it should update an existing locale.")]
  public void Given_Existing_When_SetLocale_Then_Updated()
  {
    _content.SetLocale(_english, _content.Invariant);
    _content.PublishLocale(_english);
    _content.ClearChanges();

    _content.SetLocale(_english, new ContentLocale(_content.Invariant.UniqueName, new DisplayName("Acura Integra Type S HRC Prototype Debuts at Monterey Car Week")));
    Assert.Contains(_content.Changes, change => change is ContentLocaleChanged changed && changed.LanguageId == _english.Id);

    AssertStatus(_english, ContentStatus.Published);
  }

  [Fact(DisplayName = "TryGetLocale: it should return null when the locale was not found.")]
  public void Given_NoLocale_When_TryGetLocale_Then_NullReturned()
  {
    Assert.Null(_content.TryGetLocale(_english));
  }

  [Fact(DisplayName = "TryGetLocale: it should return the locale found.")]
  public void Given_LocaleFound_When_TryGetLocale_Then_LocaleReturned()
  {
    _content.SetLocale(_english, _content.Invariant);

    ContentLocale? locale = _content.TryGetLocale(_english);
    Assert.NotNull(locale);
    Assert.Same(_content.Invariant, locale);
  }

  [Fact(DisplayName = "Unpublish: it should not do anything when the invariant and all locales are not published.")]
  public void Given_NonePublished_When_Unpublish_Then_NoChange()
  {
    _content.ClearChanges();

    _content.Unpublish();
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);
  }

  [Fact(DisplayName = "Unpublish: it should unpublish published invariant & locales.")]
  public void Given_SomePublished_When_Unpublish_Then_Unpublished()
  {
    _content.PublishInvariant();

    _content.SetLocale(_english, _content.Invariant);
    _content.PublishLocale(_english);

    _content.SetLocale(_french, _content.Invariant);

    _content.ClearChanges();
    _content.Unpublish();
    Assert.Equal(2, _content.Changes.Count);
    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished published && published.LanguageId == null);
    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished published && published.LanguageId == _english.Id);
  }

  [Fact(DisplayName = "UnpublishInvariant: it should not do anything when the invariant is not published.")]
  public void Given_NotPublished_When_UnpublishInvariant_Then_NoChange()
  {
    _content.ClearChanges();
    _content.UnpublishInvariant();
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);
  }

  [Fact(DisplayName = "UnpublishInvariant: it should unpublish the invariant when it is published.")]
  public void Given_Published_When_UnpublishInvariant_Then_Unpublished()
  {
    _content.PublishInvariant();

    _content.UnpublishInvariant();
    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished unpublished && unpublished.LanguageId == null);
    AssertStatus(language: null, status: null);

    _content.ClearChanges();
    _content.PublishInvariant();
    _content.SetInvariant(new ContentLocale(_content.Invariant.UniqueName, new DisplayName("Acura Integra Type S HRC Prototype Debuts at Monterey Car Week")));
    _content.UnpublishInvariant();
    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished unpublished && unpublished.LanguageId == null);
    AssertStatus(language: null, status: null);
  }

  [Fact(DisplayName = "UnpublishLocale: it should not do anything when the locale is not published.")]
  public void Given_NotPublished_When_UnpublishLocale_Then_NoChange()
  {
    _content.SetLocale(_english, _content.Invariant);

    _content.ClearChanges();
    Assert.True(_content.UnpublishLocale(_english));
    Assert.False(_content.HasChanges);
    Assert.Empty(_content.Changes);
  }

  [Fact(DisplayName = "UnpublishLocale: it should return false when the locale could not be found.")]
  public void Given_NoLocale_When_UnpublishLocale_Then_FalseReturned()
  {
    Assert.False(_content.UnpublishLocale(_english));
  }

  [Fact(DisplayName = "UnpublishLocale: it should unpublish the locale when it is published.")]
  public void Given_Published_When_UnpublishLocale_Then_Unpublished()
  {
    _content.SetLocale(_english, _content.Invariant);
    Assert.True(_content.PublishLocale(_english));

    _content.UnpublishLocale(_english);
    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished unpublished && unpublished.LanguageId == _english.Id);
    AssertStatus(_english, status: null);

    _content.ClearChanges();
    _content.PublishLocale(_english);
    _content.SetLocale(_english, new ContentLocale(_content.Invariant.UniqueName, new DisplayName("Acura Integra Type S HRC Prototype Debuts at Monterey Car Week")));
    Assert.True(_content.UnpublishLocale(_english));
    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished unpublished && unpublished.LanguageId == _english.Id);
    AssertStatus(_english, status: null);
  }

  private void AssertStatus(Language? language, ContentStatus? status)
  {
    if (language == null)
    {
      FieldInfo? field = _content.GetType().GetField("_invariantStatus", BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.NotNull(field);

      ContentStatus? value = field.GetValue(_content) as ContentStatus?;
      Assert.Equal(status, value);
    }
    else
    {
      FieldInfo? field = _content.GetType().GetField("_localeStatuses", BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.NotNull(field);

      Dictionary<LanguageId, ContentStatus>? value = field.GetValue(_content) as Dictionary<LanguageId, ContentStatus>;
      Assert.NotNull(value);
      if (status.HasValue)
      {
        Assert.True(value.TryGetValue(language.Id, out ContentStatus localeStatus) && localeStatus == status);
      }
      else
      {
        Assert.False(value.ContainsKey(language.Id));
      }
    }
  }
}
