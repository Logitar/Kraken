using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Templates.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Commands;

public record CreateOrReplaceTemplateResult(TemplateModel? Template = null, bool Created = false);

public record CreateOrReplaceTemplateCommand(Guid? Id, CreateOrReplaceTemplatePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceTemplateResult>;

internal class CreateOrReplaceTemplateCommandHandler : IRequestHandler<CreateOrReplaceTemplateCommand, CreateOrReplaceTemplateResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITemplateManager _templateManager;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public CreateOrReplaceTemplateCommandHandler(
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

  public async Task<CreateOrReplaceTemplateResult> Handle(CreateOrReplaceTemplateCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceTemplatePayload payload = command.Payload;
    new CreateOrReplaceTemplateValidator().ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    TemplateId templateId = TemplateId.NewId(realmId);
    Template? template = null;
    if (command.Id.HasValue)
    {
      templateId = new(realmId, command.Id.Value);
      template = await _templateRepository.LoadAsync(templateId, cancellationToken);
    }

    Identifier uniqueKey = new(payload.UniqueKey);
    Subject subject = new(payload.Subject);
    TemplateContent content = new(payload.Content);
    ActorId? actorId = _applicationContext.ActorId;

    bool created = false;
    if (template == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceTemplateResult();
      }

      template = new(uniqueKey, subject, content, actorId, templateId);
      created = true;
    }

    Template reference = (command.Version.HasValue
      ? await _templateRepository.LoadAsync(template.Id, command.Version.Value, cancellationToken)
      : null) ?? template;

    if (reference.UniqueKey != uniqueKey)
    {
      template.SetUniqueKey(uniqueKey, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      template.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      template.Description = description;
    }

    if (reference.Subject != subject)
    {
      template.Subject = subject;
    }
    if (reference.Content != content)
    {
      template.Content = content;
    }

    template.Update(actorId);
    await _templateManager.SaveAsync(template, cancellationToken);

    TemplateModel model = await _templateQuerier.ReadAsync(template, cancellationToken);
    return new CreateOrReplaceTemplateResult(model, created);
  }
}
