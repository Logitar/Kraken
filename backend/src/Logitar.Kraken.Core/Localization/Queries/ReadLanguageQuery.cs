using Logitar.Kraken.Contracts.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Localization.Queries;

/// <exception cref="TooManyResultsException"></exception>
public record ReadLanguageQuery(Guid? Id, string? Locale, bool IsDefault) : IRequest<LanguageModel?>;

internal class ReadLanguageQueryHandler : IRequestHandler<ReadLanguageQuery, LanguageModel?>
{
  private readonly ILanguageQuerier _languageQuerier;

  public ReadLanguageQueryHandler(ILanguageQuerier languageQuerier)
  {
    _languageQuerier = languageQuerier;
  }

  public async Task<LanguageModel?> Handle(ReadLanguageQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, LanguageModel> languages = new(capacity: 3);

    if (query.Id.HasValue)
    {
      LanguageModel? language = await _languageQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (language != null)
      {
        languages[language.Id] = language;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.Locale))
    {
      LanguageModel? language = await _languageQuerier.ReadAsync(query.Locale, cancellationToken);
      if (language != null)
      {
        languages[language.Id] = language;
      }
    }
    if (query.IsDefault)
    {
      LanguageModel language = await _languageQuerier.ReadDefaultAsync(cancellationToken);
      languages[language.Id] = language;
    }

    if (languages.Count > 1)
    {
      throw TooManyResultsException<LanguageModel>.ExpectedSingle(languages.Count);
    }

    return languages.Values.SingleOrDefault();
  }
}
