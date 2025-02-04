using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Core.Templates.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Commands;

public record UpdateTemplateCommand(Guid Id, UpdateTemplatePayload Payload) : Activity, IRequest<TemplateModel?>;

internal class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, TemplateModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITemplateManager _templateManager;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public UpdateTemplateCommandHandler(
    IApplicationContext applicationContext,
    ITemplateManager templateManager,
    ITemplateQuerier templateQuerier,
    ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _templateManager = templateManager;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<TemplateModel?> Handle(UpdateTemplateCommand command, CancellationToken cancellationToken)
  {
    UpdateTemplatePayload payload = command.Payload;
    new UpdateTemplateValidator().ValidateAndThrow(payload);

    TemplateId templateId = new(_applicationContext.RealmId, command.Id);
    Template? template = await _templateRepository.LoadAsync(templateId, cancellationToken);
    if (template == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueKey))
    {
      Identifier uniqueKey = new(payload.UniqueKey);
      template.SetUniqueKey(uniqueKey);
    }
    if (payload.DisplayName != null)
    {
      template.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      template.Description = Description.TryCreate(payload.Description.Value);
    }

    if (!string.IsNullOrWhiteSpace(payload.Subject))
    {
      template.Subject = new Subject(payload.Subject);
    }
    if (payload.Content != null)
    {
      template.Content = new TemplateContent(payload.Content);
    }

    template.Update(actorId);
    await _templateManager.SaveAsync(template, cancellationToken);

    return await _templateQuerier.ReadAsync(template, cancellationToken);
  }
}
