using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Templates.Commands;
using Logitar.Kraken.Core.Templates.Queries;
using Logitar.Kraken.Web.Models.Template;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/templates")]
public class TemplateController : ControllerBase
{
  private readonly IMediator _mediator;

  public TemplateController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<TemplateModel>> CreateAsync([FromBody] CreateOrReplaceTemplatePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTemplateCommand command = new(Id: null, payload, Version: null);
    CreateOrReplaceTemplateResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<TemplateModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteTemplateCommand command = new(id);
    TemplateModel? template = await _mediator.Send(command, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TemplateModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadTemplateQuery query = new(id, UniqueKey: null);
    TemplateModel? template = await _mediator.Send(query, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpGet("key:{uniqueKey}")]
  public async Task<ActionResult<UserModel>> ReadAsync(string uniqueKey, CancellationToken cancellationToken)
  {
    ReadTemplateQuery query = new(Id: null, uniqueKey);
    TemplateModel? template = await _mediator.Send(query, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TemplateModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceTemplateCommand command = new(id, payload, version);
    CreateOrReplaceTemplateResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<TemplateModel>>> SearchAsync([FromQuery] SearchTemplatesParameters parameters, CancellationToken cancellationToken)
  {
    SearchTemplatesPayload payload = parameters.ToPayload();
    SearchTemplatesQuery query = new(payload);
    SearchResults<TemplateModel> templates = await _mediator.Send(query, cancellationToken);
    return Ok(templates);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<TemplateModel>> UpdateAsync(Guid id, [FromBody] UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    UpdateTemplateCommand command = new(id, payload);
    TemplateModel? template = await _mediator.Send(command, cancellationToken);
    return template == null ? NotFound() : Ok(template);
  }

  private ActionResult<TemplateModel> ToActionResult(CreateOrReplaceTemplateResult result)
  {
    if (result.Template == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/templates/{result.Template.Id}", UriKind.Absolute);
      return Created(location, result.Template);
    }
    return Ok(result.Template);
  }
}
