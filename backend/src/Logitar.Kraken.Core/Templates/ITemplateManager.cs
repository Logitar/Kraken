namespace Logitar.Kraken.Core.Templates;

public interface ITemplateManager
{
  Task SaveAsync(Template template, CancellationToken cancellationToken = default);
}
