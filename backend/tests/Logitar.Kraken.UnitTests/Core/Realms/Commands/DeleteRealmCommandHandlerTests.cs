using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Messages;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Sessions;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Users;
using Moq;

namespace Logitar.Kraken.Core.Realms.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteRealmCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApiKeyRepository> _apiKeyRepository = new();
  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<IDictionaryRepository> _dictionaryRepository = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IMessageRepository> _messageRepository = new();
  private readonly Mock<IOneTimePasswordRepository> _oneTimePasswordRepository = new();
  private readonly Mock<IRealmQuerier> _realmQuerier = new();
  private readonly Mock<IRealmRepository> _realmRepository = new();
  private readonly Mock<IRoleRepository> _roleRepository = new();
  private readonly Mock<ISenderRepository> _senderRepository = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<ITemplateRepository> _templateRepository = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly DeleteRealmCommandHandler _handler;

  private readonly Realm _realm = new(new Slug("the-new-world"));

  public DeleteRealmCommandHandlerTests()
  {
    _handler = new(
      _apiKeyRepository.Object,
      _applicationContext.Object,
      _contentRepository.Object,
      _contentTypeRepository.Object,
      _dictionaryRepository.Object,
      _fieldTypeRepository.Object,
      _languageRepository.Object,
      _messageRepository.Object,
      _oneTimePasswordRepository.Object,
      _realmQuerier.Object,
      _realmRepository.Object,
      _roleRepository.Object,
      _senderRepository.Object,
      _sessionRepository.Object,
      _templateRepository.Object,
      _userRepository.Object);
  }

  [Fact(DisplayName = "It should delete an existing realm.")]
  public async Task Given_Found_When_Handle_Then_Deleted()
  {
    _realmRepository.Setup(x => x.LoadAsync(_realm.Id, _cancellationToken)).ReturnsAsync(_realm);

    RealmModel model = new();
    _realmQuerier.Setup(x => x.ReadAsync(_realm, _cancellationToken)).ReturnsAsync(model);

    DeleteRealmCommand command = new(_realm.Id.ToGuid());
    RealmModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(_realm.IsDeleted);
    _realmRepository.Verify(x => x.SaveAsync(_realm, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the realm could not be found.")]
  public async Task Given_NotFound_When_Handle_Then_NullReturned()
  {
    DeleteRealmCommand command = new(_realm.Id.ToGuid());
    RealmModel? realm = await _handler.Handle(command, _cancellationToken);
    Assert.Null(realm);
  }
}
