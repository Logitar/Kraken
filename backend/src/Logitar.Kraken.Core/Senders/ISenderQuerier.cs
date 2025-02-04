using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders;

public interface ISenderQuerier
{
  Task<SenderId?> FindDefaultIdAsync(SenderType type, CancellationToken cancellationToken = default);

  Task<SenderModel> ReadAsync(Sender sender, CancellationToken cancellationToken = default);
  Task<SenderModel?> ReadAsync(SenderId id, CancellationToken cancellationToken = default);
  Task<SenderModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SenderModel?> ReadDefaultAsync(SenderType type, CancellationToken cancellationToken = default);

  Task<SearchResults<SenderModel>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken = default);
}
