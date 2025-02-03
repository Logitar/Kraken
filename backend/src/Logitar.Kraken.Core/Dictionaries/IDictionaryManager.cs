namespace Logitar.Kraken.Core.Dictionaries;

public interface IDictionaryManager
{
  Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
}
