using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/contents/types/{contentTypeId}/fields")]
public class FieldDefinitionController : ControllerBase
{
  private readonly IMediator _mediator;

  public FieldDefinitionController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<ContentTypeModel>> CreateFieldAsync(Guid contentTypeId, [FromBody] CreateOrReplaceFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new CreateOrReplaceFieldDefinitionCommand(contentTypeId, FieldId: null, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpDelete("{fieldId}")]
  public async Task<ActionResult<ContentTypeModel>> RemoveFieldAsync(Guid contentTypeId, Guid fieldId, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new RemoveFieldDefinitionCommand(contentTypeId, fieldId), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpPut("{fieldId}")]
  public async Task<ActionResult<ContentTypeModel>> ReplaceFieldAsync(Guid contentTypeId, Guid fieldId, [FromBody] CreateOrReplaceFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new CreateOrReplaceFieldDefinitionCommand(contentTypeId, fieldId, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpPatch("{fieldId}")]
  public async Task<ActionResult<ContentTypeModel>> UpdateFieldAsync(Guid contentTypeId, Guid fieldId, [FromBody] UpdateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new UpdateFieldDefinitionCommand(contentTypeId, fieldId, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }
}
