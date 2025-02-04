using Logitar.Kraken.Contracts.Templates;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Commands;

public record DeleteTemplateCommand(Guid Id) : Activity, IRequest<TemplateModel?>;

internal class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, TemplateModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public DeleteTemplateCommandHandler(IApplicationContext applicationContext, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<TemplateModel?> Handle(DeleteTemplateCommand command, CancellationToken cancellationToken)
  {
    TemplateId templateId = new(_applicationContext.RealmId, command.Id);
    Template? template = await _templateRepository.LoadAsync(templateId, cancellationToken);
    if (template == null)
    {
      return null;
    }
    TemplateModel result = await _templateQuerier.ReadAsync(template, cancellationToken);

    template.Delete(_applicationContext.ActorId);
    await _templateRepository.SaveAsync(template, cancellationToken);

    return result;
  }
}
