using Logitar.Kraken.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Commands;

public record DeleteDictionaryCommand(Guid Id) : Activity, IRequest<DictionaryModel?>;

internal class DeleteDictionaryCommandHandler : IRequestHandler<DeleteDictionaryCommand, DictionaryModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public DeleteDictionaryCommandHandler(IApplicationContext applicationContext, IDictionaryQuerier dictionaryQuerier, IDictionaryRepository dictionaryRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task<DictionaryModel?> Handle(DeleteDictionaryCommand command, CancellationToken cancellationToken)
  {
    DictionaryId dictionaryId = new(command.Id, _applicationContext.RealmId);
    Dictionary? dictionary = await _dictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
    if (dictionary == null)
    {
      return null;
    }
    DictionaryModel result = await _dictionaryQuerier.ReadAsync(dictionary, cancellationToken);

    dictionary.Delete(_applicationContext.ActorId);
    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return result;
  }
}
