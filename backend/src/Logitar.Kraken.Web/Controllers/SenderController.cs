using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders.Commands;
using Logitar.Kraken.Core.Senders.Queries;
using Logitar.Kraken.Web.Models.Sender;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/senders")]
public class SenderController : ControllerBase
{
  private readonly IMediator _mediator;

  public SenderController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<SenderModel>> CreateAsync([FromBody] CreateOrReplaceSenderPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSenderCommand command = new(Id: null, payload, Version: null);
    CreateOrReplaceSenderResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<SenderModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteSenderCommand command = new(id);
    SenderModel? sender = await _mediator.Send(command, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<SenderModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSenderQuery query = new(id, Type: null);
    SenderModel? sender = await _mediator.Send(query, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpGet("default/{type}")]
  public async Task<ActionResult<SenderModel>> ReadDefaultAsync(SenderType type, CancellationToken cancellationToken)
  {
    ReadSenderQuery query = new(Id: null, type);
    SenderModel? sender = await _mediator.Send(query, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<SenderModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceSenderPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceSenderCommand command = new(id, payload, version);
    CreateOrReplaceSenderResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<SenderModel>>> SearchAsync([FromQuery] SearchSendersParameters parameters, CancellationToken cancellationToken)
  {
    SearchSendersPayload payload = parameters.ToPayload();
    SearchSendersQuery query = new(payload);
    SearchResults<SenderModel> senders = await _mediator.Send(query, cancellationToken);
    return Ok(senders);
  }

  [HttpPatch("{id}/default")]
  public async Task<ActionResult<SenderModel>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    SetDefaultSenderCommand command = new(id);
    SenderModel? sender = await _mediator.Send(command, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<SenderModel>> UpdateAsync(Guid id, [FromBody] UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    UpdateSenderCommand command = new(id, payload);
    SenderModel? sender = await _mediator.Send(command, cancellationToken);
    return sender == null ? NotFound() : Ok(sender);
  }

  private ActionResult<SenderModel> ToActionResult(CreateOrReplaceSenderResult result)
  {
    if (result.Sender == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/senders/{result.Sender.Id}", UriKind.Absolute);
      return Created(location, result.Sender);
    }
    return Ok(result.Sender);
  }
}
