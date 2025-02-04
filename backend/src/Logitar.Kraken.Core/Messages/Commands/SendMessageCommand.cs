using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Messages.Validators;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Users;
using MediatR;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Messages.Commands;

public record SendMessageCommand(SendMessagePayload Payload) : Activity, IRequest<SentMessages>;

internal class SendMessageInternalCommandHandler : IRequestHandler<SendMessageCommand, SentMessages>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMessageManager _messageManager;
  private readonly IMessageRepository _messageRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  public SendMessageInternalCommandHandler(
    IApplicationContext applicationContext,
    IDictionaryRepository dictionaryRepository,
    ILanguageRepository languageRepository,
    IMessageManager messageManager,
    IMessageRepository messageRepository,
    ISenderRepository senderRepository,
    ITemplateRepository templateRepository,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryRepository = dictionaryRepository;
    _languageRepository = languageRepository;
    _messageRepository = messageRepository;
    _messageManager = messageManager;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
    _userRepository = userRepository;
  }

  public async Task<SentMessages> Handle(SendMessageCommand command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    RealmId? realmId = _applicationContext.RealmId;
    Locale defaultLocale = (await _languageRepository.LoadDefaultAsync(realmId, cancellationToken)).Locale;

    SendMessagePayload payload = command.Payload;
    Sender sender = await ResolveSenderAsync(realmId, payload, cancellationToken);
    new SendMessageValidator(sender.Type).ValidateAndThrow(payload);

    Recipients allRecipients = await ResolveRecipientsAsync(realmId, payload, sender.Type, cancellationToken);
    Template template = await ResolveTemplateAsync(realmId, payload, sender.Type, cancellationToken);

    IReadOnlyDictionary<Locale, Dictionary> allDictionaries = await ResolveDictionariesAsync(realmId, cancellationToken);
    bool ignoreUserLocale = payload.IgnoreUserLocale;
    Locale? targetLocale = Locale.TryCreate(payload.Locale);
    Dictionaries defaultDictionaries = new(allDictionaries, targetLocale, defaultLocale);

    Variables variables = new(payload.Variables);
    IReadOnlyDictionary<string, string> variableDictionary = variables.AsDictionary();

    List<Message> messages = new(capacity: allRecipients.To.Count);
    foreach (Recipient recipient in allRecipients.To)
    {
      MessageId messageId = MessageId.NewId(realmId);

      Dictionaries dictionaries = (payload.IgnoreUserLocale || recipient.User?.Locale == null)
        ? defaultDictionaries : new(allDictionaries, recipient.User.Locale, defaultLocale);

      Subject subject = new(dictionaries.Translate(template.Subject.Value));
      Locale? locale = dictionaries.TargetLocale ?? dictionaries.DefaultLocale;
      TemplateContent body = await _messageManager.CompileAsync(messageId, template, dictionaries, locale, recipient.User, variables, cancellationToken);
      IReadOnlyCollection<Recipient> recipients = [recipient, .. allRecipients.CC, .. allRecipients.Bcc];

      Message message = new(subject, body, recipients, sender, template, ignoreUserLocale, locale, variableDictionary, payload.IsDemo, actorId, messageId);
      messages.Add(message);

      await _messageManager.SendAsync(message, sender, actorId, cancellationToken);
    }
    await _messageRepository.SaveAsync(messages, cancellationToken);

    return new SentMessages(messages.Select(x => x.Id.EntityId));
  }

  private async Task<IReadOnlyDictionary<Locale, Dictionary>> ResolveDictionariesAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Dictionary> dictionaries = await _dictionaryRepository.LoadAsync(realmId, cancellationToken);

    HashSet<LanguageId> languageIds = dictionaries.Select(dictionary => dictionary.LanguageId).ToHashSet();
    Dictionary<LanguageId, Language> languages = (await _languageRepository.LoadAsync(languageIds, cancellationToken))
      .ToDictionary(x => x.Id, x => x);

    Dictionary<Locale, Dictionary> dictionariesByLocale = new(capacity: dictionaries.Count);
    foreach (Dictionary dictionary in dictionaries)
    {
      if (languages.TryGetValue(dictionary.LanguageId, out Language? language))
      {
        dictionariesByLocale[language.Locale] = dictionary;
      }
    }
    return dictionariesByLocale.AsReadOnly();
  }

  private async Task<Recipients> ResolveRecipientsAsync(RealmId? realmId, SendMessagePayload payload, SenderType senderType, CancellationToken cancellationToken)
  {
    List<Recipient> recipients = new(capacity: payload.Recipients.Count);

    HashSet<UserId> userIds = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        userIds.Add(new UserId(realmId, recipient.UserId.Value));
      }
    }

    Dictionary<Guid, User> users = new(recipients.Capacity);
    if (userIds.Count > 0)
    {
      IEnumerable<User> allUsers = await _userRepository.LoadAsync(userIds, cancellationToken);
      foreach (User user in allUsers)
      {
        users[user.EntityId] = user;
      }
    }

    List<Guid> missingUsers = new(recipients.Capacity);
    List<Guid> missingContacts = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        if (users.TryGetValue(recipient.UserId.Value, out User? user))
        {
          switch (senderType)
          {
            case SenderType.Email:
              if (user.Email == null)
              {
                missingContacts.Add(recipient.UserId.Value);
                continue;
              }
              break;
            case SenderType.Phone:
              if (user.Phone == null)
              {
                missingContacts.Add(recipient.UserId.Value);
                continue;
              }
              break;
            default:
              throw new SenderTypeNotSupportedException(senderType);
          }

          recipients.Add(new Recipient(user, recipient.Type));
        }
        else
        {
          missingUsers.Add(recipient.UserId.Value);
        }
      }
      else
      {
        recipients.Add(new Recipient(recipient.Type, recipient.Address, recipient.DisplayName, recipient.PhoneNumber));
      }
    }
    if (missingUsers.Count > 0)
    {
      throw new UsersNotFoundException(realmId, missingUsers, nameof(payload.Recipients));
    }
    else if (missingContacts.Count > 0)
    {
      throw new MissingRecipientContactsException(realmId, missingContacts, nameof(payload.Recipients));
    }

    return new Recipients(recipients);
  }

  private async Task<Sender> ResolveSenderAsync(RealmId? realmId, SendMessagePayload payload, CancellationToken cancellationToken)
  {
    if (payload.SenderId.HasValue)
    {
      SenderId senderId = new(realmId, payload.SenderId.Value);
      return await _senderRepository.LoadAsync(senderId, cancellationToken)
        ?? throw new SenderNotFoundException(senderId, nameof(payload.SenderId));
    }

    SenderType senderType = SenderType.Email; // TODO(fpion): Email or Phone
    return await _senderRepository.LoadDefaultAsync(realmId, senderType, cancellationToken)
      ?? throw new NoDefaultSenderException(realmId, senderType);
  }

  private async Task<Template> ResolveTemplateAsync(RealmId? realmId, SendMessagePayload payload, SenderType senderType, CancellationToken cancellationToken)
  {
    Template? template = null;
    if (Guid.TryParse(payload.Template, out Guid id))
    {
      TemplateId templateId = new(realmId, id);
      template = await _templateRepository.LoadAsync(templateId, cancellationToken);
    }

    if (template == null)
    {
      try
      {
        Identifier uniqueKey = new(payload.Template);
        template = await _templateRepository.LoadAsync(realmId, uniqueKey, cancellationToken);
      }
      catch (ValidationException)
      {
      }
    }

    if (template == null)
    {
      throw new TemplateNotFoundException(realmId, payload.Template, nameof(payload.Template));
    }
    else if (senderType == SenderType.Phone && template.Content.Type != MediaTypeNames.Text.Plain)
    {
      throw new InvalidSmsMessageContentTypeException(template.Content.Type, nameof(payload.Template));
    }

    return template;
  }
}
