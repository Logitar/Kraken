using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Contents.Commands;
using Logitar.Kraken.Core.Contents.Queries;
using Logitar.Kraken.Web.Models.ContentType;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/contents/types")]
public class ContentTypeController : ControllerBase
{
  private readonly IMediator _mediator;

  public ContentTypeController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<ContentTypeModel>> CreateAsync([FromBody] CreateOrReplaceContentTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypeResult result = await _mediator.Send(new CreateOrReplaceContentTypeCommand(Id: null, payload, Version: null), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ContentTypeModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new ReadContentTypeQuery(id, UniqueName: null), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<ContentTypeModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new ReadContentTypeQuery(Id: null, uniqueName), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ContentTypeModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceContentTypePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypeResult result = await _mediator.Send(new CreateOrReplaceContentTypeCommand(id, payload, version), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ContentTypeModel>>> SearchAsync([FromQuery] SearchContentTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<ContentTypeModel> contentTypes = await _mediator.Send(new SearchContentTypesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contentTypes);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ContentTypeModel>> UpdateAsync(Guid id, [FromBody] UpdateContentTypePayload payload, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new UpdateContentTypeCommand(id, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  private ActionResult<ContentTypeModel> ToActionResult(CreateOrReplaceContentTypeResult result)
  {
    if (result.ContentType == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/contents/types/{result.ContentType.Id}", UriKind.Absolute);
      return Created(location, result.ContentType);
    }

    return Ok(result.ContentType);
  }
}
