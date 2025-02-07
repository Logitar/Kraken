using Logitar.EventSourcing;
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
using MediatR;

namespace Logitar.Kraken.Core.Realms.Commands;

public record DeleteRealmCommand(Guid Id) : Activity, IRequest<RealmModel?>;

internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, RealmModel?>
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMessageRepository _messageRepository;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;
  private readonly IRoleRepository _roleRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ISessionRepository _sessionRepository;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  public DeleteRealmCommandHandler(
    IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext,
    IContentRepository contentRepository,
    IContentTypeRepository contentTypeRepository,
    IDictionaryRepository dictionaryRepository,
    IFieldTypeRepository fieldTypeRepository,
    ILanguageRepository languageRepository,
    IMessageRepository messageRepository,
    IOneTimePasswordRepository oneTimePasswordRepository,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository,
    IRoleRepository roleRepository,
    ISenderRepository senderRepository,
    ISessionRepository sessionRepository,
    ITemplateRepository templateRepository,
    IUserRepository userRepository)
  {
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _contentRepository = contentRepository;
    _contentTypeRepository = contentTypeRepository;
    _dictionaryRepository = dictionaryRepository;
    _fieldTypeRepository = fieldTypeRepository;
    _languageRepository = languageRepository;
    _messageRepository = messageRepository;
    _oneTimePasswordRepository = oneTimePasswordRepository;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
    _roleRepository = roleRepository;
    _senderRepository = senderRepository;
    _sessionRepository = sessionRepository;
    _templateRepository = templateRepository;
    _userRepository = userRepository;
  }

  public async Task<RealmModel?> Handle(DeleteRealmCommand command, CancellationToken cancellationToken)
  {
    RealmId realmId = new(command.Id);
    Realm? realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    if (realm == null)
    {
      return null;
    }
    RealmModel result = await _realmQuerier.ReadAsync(realm, cancellationToken);

    ActorId? actorId = _applicationContext.ActorId;

    await DeleteContentsAsync(realmId, actorId, cancellationToken);
    await DeleteContentTypesAsync(realmId, actorId, cancellationToken);
    await DeleteFieldTypesAsync(realmId, actorId, cancellationToken);

    await DeleteMessagesAsync(realmId, actorId, cancellationToken);
    await DeleteTemplatesAsync(realmId, actorId, cancellationToken);
    await DeleteSendersAsync(realmId, actorId, cancellationToken);

    await DeleteDictionariesAsync(realmId, actorId, cancellationToken);
    await DeleteLanguagesAsync(realmId, actorId, cancellationToken);

    await DeleteSessionsAsync(realmId, actorId, cancellationToken);
    await DeleteOneTimePasswordsAsync(realmId, actorId, cancellationToken);
    await DeleteUsersAsync(realmId, actorId, cancellationToken);
    await DeleteApiKeysAsync(realmId, actorId, cancellationToken);
    await DeleteRolesAsync(realmId, actorId, cancellationToken);

    realm.Delete(actorId);
    await _realmRepository.SaveAsync(realm, cancellationToken);

    return result;
  }

  private async Task DeleteApiKeysAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<ApiKey> apiKeys = await _apiKeyRepository.LoadAsync(realmId, cancellationToken);
    foreach (ApiKey apiKey in apiKeys)
    {
      apiKey.Delete(actorId);
    }
    await _apiKeyRepository.SaveAsync(apiKeys, cancellationToken);
  }

  private async Task DeleteContentsAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Content> contents = await _contentRepository.LoadAsync(realmId, cancellationToken);
    foreach (Content content in contents)
    {
      content.Delete(actorId);
    }
    await _contentRepository.SaveAsync(contents, cancellationToken);
  }

  private async Task DeleteContentTypesAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<ContentType> contentTypes = await _contentTypeRepository.LoadAsync(realmId, cancellationToken);
    foreach (ContentType contentType in contentTypes)
    {
      contentType.Delete(actorId);
    }
    await _contentTypeRepository.SaveAsync(contentTypes, cancellationToken);
  }

  private async Task DeleteDictionariesAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Dictionary> dictionaries = await _dictionaryRepository.LoadAsync(realmId, cancellationToken);
    foreach (Dictionary dictionary in dictionaries)
    {
      dictionary.Delete(actorId);
    }
    await _dictionaryRepository.SaveAsync(dictionaries, cancellationToken);
  }

  private async Task DeleteFieldTypesAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<FieldType> fieldTypes = await _fieldTypeRepository.LoadAsync(realmId, cancellationToken);
    foreach (FieldType fieldType in fieldTypes)
    {
      fieldType.Delete(actorId);
    }
    await _fieldTypeRepository.SaveAsync(fieldTypes, cancellationToken);
  }

  private async Task DeleteLanguagesAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Language> languages = await _languageRepository.LoadAsync(realmId, cancellationToken);
    foreach (Language language in languages)
    {
      language.Delete(actorId);
    }
    await _languageRepository.SaveAsync(languages, cancellationToken);
  }

  private async Task DeleteMessagesAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Message> messages = await _messageRepository.LoadAsync(realmId, cancellationToken);
    foreach (Message message in messages)
    {
      message.Delete(actorId);
    }
    await _messageRepository.SaveAsync(messages, cancellationToken);
  }

  private async Task DeleteOneTimePasswordsAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<OneTimePassword> oneTimePasswords = await _oneTimePasswordRepository.LoadAsync(realmId, cancellationToken);
    foreach (OneTimePassword oneTimePassword in oneTimePasswords)
    {
      oneTimePassword.Delete(actorId);
    }
    await _oneTimePasswordRepository.SaveAsync(oneTimePasswords, cancellationToken);
  }

  private async Task DeleteRolesAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Role> roles = await _roleRepository.LoadAsync(realmId, cancellationToken);
    foreach (Role role in roles)
    {
      role.Delete(actorId);
    }
    await _roleRepository.SaveAsync(roles, cancellationToken);
  }

  private async Task DeleteSendersAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Sender> senders = await _senderRepository.LoadAsync(realmId, cancellationToken);
    foreach (Sender sender in senders)
    {
      sender.Delete(actorId);
    }
    await _senderRepository.SaveAsync(senders, cancellationToken);
  }

  private async Task DeleteSessionsAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Session> sessions = await _sessionRepository.LoadAsync(realmId, cancellationToken);
    foreach (Session session in sessions)
    {
      session.Delete(actorId);
    }
    await _sessionRepository.SaveAsync(sessions, cancellationToken);
  }

  private async Task DeleteTemplatesAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Template> templates = await _templateRepository.LoadAsync(realmId, cancellationToken);
    foreach (Template template in templates)
    {
      template.Delete(actorId);
    }
    await _templateRepository.SaveAsync(templates, cancellationToken);
  }

  private async Task DeleteUsersAsync(RealmId realmId, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<User> users = await _userRepository.LoadAsync(realmId, cancellationToken);
    foreach (User user in users)
    {
      user.Delete(actorId);
    }
    await _userRepository.SaveAsync(users, cancellationToken);
  }
}
