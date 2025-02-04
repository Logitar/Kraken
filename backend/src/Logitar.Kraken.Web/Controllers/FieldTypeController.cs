using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Fields.Commands;
using Logitar.Kraken.Core.Fields.Queries;
using Logitar.Kraken.Web.Models.FieldType;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/fields/types")]
public class FieldTypeController : ControllerBase
{
  private readonly IMediator _mediator;

  public FieldTypeController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<FieldTypeModel>> CreateAsync([FromBody] CreateOrReplaceFieldTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypeResult result = await _mediator.Send(new CreateOrReplaceFieldTypeCommand(Id: null, payload, Version: null), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<FieldTypeModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _mediator.Send(new ReadFieldTypeQuery(id, UniqueName: null), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<FieldTypeModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _mediator.Send(new ReadFieldTypeQuery(Id: null, uniqueName), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<FieldTypeModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceFieldTypePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypeResult result = await _mediator.Send(new CreateOrReplaceFieldTypeCommand(id, payload, version), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<FieldTypeModel>>> SearchAsync([FromQuery] SearchFieldTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<FieldTypeModel> fieldTypes = await _mediator.Send(new SearchFieldTypesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(fieldTypes);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<FieldTypeModel>> UpdateAsync(Guid id, [FromBody] UpdateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _mediator.Send(new UpdateFieldTypeCommand(id, payload), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  private ActionResult<FieldTypeModel> ToActionResult(CreateOrReplaceFieldTypeResult result)
  {
    if (result.FieldType == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/fields/types/{result.FieldType.Id}", UriKind.Absolute);
      return Created(location, result.FieldType);
    }
    return Ok(result.FieldType);
  }
}
