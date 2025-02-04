using Logitar.Kraken.Core.Templates.Events;

namespace Logitar.Kraken.Core.Templates;

internal class TemplateManager : ITemplateManager
{
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public TemplateManager(ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task SaveAsync(Template template, CancellationToken cancellationToken)
  {
    bool hasUniqueKeyChanged = template.Changes.Any(change => change is TemplateCreated || change is TemplateUniqueKeyChanged);
    if (hasUniqueKeyChanged)
    {
      TemplateId? conflictId = await _templateQuerier.FindIdAsync(template.UniqueKey, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(template.Id))
      {
        throw new UniqueKeyAlreadyUsedException(template, conflictId.Value);
      }
    }

    await _templateRepository.SaveAsync(template, cancellationToken);
  }
}
