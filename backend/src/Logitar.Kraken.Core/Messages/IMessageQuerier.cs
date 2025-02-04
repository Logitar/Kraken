using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Core.Messages;

public interface IMessageQuerier
{
  Task<MessageModel> ReadAsync(Message message, CancellationToken cancellationToken = default);
  Task<MessageModel?> ReadAsync(MessageId id, CancellationToken cancellationToken = default);
  Task<MessageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<MessageModel>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken = default);
}
