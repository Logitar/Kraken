using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Messages;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class MessageQuerier : IMessageQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<MessageEntity> _messages;

  public MessageQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _messages = context.Messages;
  }

  public async Task<MessageModel> ReadAsync(Message message, CancellationToken cancellationToken)
  {
    return await ReadAsync(message.Id, cancellationToken) ?? throw new InvalidOperationException($"The message entity 'StreamId={message.Id}' could not be found.");
  }
  public async Task<MessageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new MessageId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<MessageModel?> ReadAsync(MessageId id, CancellationToken cancellationToken)
  {
    MessageEntity? message = await _messages.AsNoTracking()
      .Include(x => x.Recipients)
      .Include(x => x.Sender)
      .Include(x => x.Template)
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return message == null ? null : await MapAsync(message, cancellationToken);
  }

  public Task<SearchResults<MessageModel>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<MessageModel> MapAsync(MessageEntity message, CancellationToken cancellationToken)
  {
    return (await MapAsync([message], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<MessageModel>> MapAsync(IEnumerable<MessageEntity> messages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = messages.SelectMany(message => message.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return messages.Select(message => mapper.ToMessage(message, _applicationContext.Realm)).ToArray();
  }
}
