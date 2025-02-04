using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Contents.Commands;
using Logitar.Kraken.Core.Contents.Queries;
using Logitar.Kraken.Web.Models.Content;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/contents")]
public class ContentController : ControllerBase
{
  private readonly IMediator _mediator;

  public ContentController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<ContentModel>> CreateAsync(
    [FromQuery(Name = "language")] Guid? languageId,
    [FromBody] CreateOrReplaceContentPayload payload,
    CancellationToken cancellationToken)
  {
    CreateOrReplaceContentResult result = await _mediator.Send(new CreateOrReplaceContentCommand(ContentId: null, languageId, payload), cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{contentId}/publish/all")]
  public async Task<ActionResult<ContentModel>> PublishAllAsync(Guid contentId, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new PublishContentCommand(contentId), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpPatch("{contentId}/publish")]
  public async Task<ActionResult<ContentModel>> PublishAsync(Guid contentId, [FromQuery(Name = "language")] Guid? languageId, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new PublishContentCommand(contentId, languageId), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ContentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new ReadContentQuery(id, Key: null), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpGet("types/{contentTypeId}/unique-name:{uniqueName}")]
  public async Task<ActionResult<ContentModel>> ReadAsync(Guid contentTypeId, string uniqueName, [FromQuery(Name = "language")] Guid? languageId, CancellationToken cancellationToken)
  {
    // TODO(fpion): contentType (Id or UniqueName)
    // TODO(fpion): language (Id or Locale code)
    ContentKey key = new(contentTypeId, languageId, uniqueName);
    ContentModel? content = await _mediator.Send(new ReadContentQuery(Id: null, key), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpPut("{contentId}")]
  public async Task<ActionResult<ContentModel>> ReplaceAsync(
    Guid contentId,
    [FromQuery(Name = "language")] Guid? languageId,
    [FromBody] CreateOrReplaceContentPayload payload,
    CancellationToken cancellationToken)
  {
    CreateOrReplaceContentResult result = await _mediator.Send(new CreateOrReplaceContentCommand(contentId, languageId, payload), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ContentLocaleModel>>> SearchAsync([FromQuery] SearchContentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<ContentLocaleModel> contents = await _mediator.Send(new SearchContentsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contents);
  }

  [HttpPatch("{contentId}/unpublish/all")]
  public async Task<ActionResult<ContentModel>> UnpublishAllAsync(Guid contentId, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new UnpublishContentCommand(contentId), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpPatch("{contentId}/unpublish")]
  public async Task<ActionResult<ContentModel>> UnpublishAsync(Guid contentId, [FromQuery(Name = "language")] Guid? languageId, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new UnpublishContentCommand(contentId, languageId), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpPatch("{contentId}")]
  public async Task<ActionResult<ContentModel>> UpdateAsync(Guid contentId, Guid? languageId, [FromBody] UpdateContentPayload payload, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new UpdateContentCommand(contentId, languageId, payload), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  private ActionResult<ContentModel> ToActionResult(CreateOrReplaceContentResult result)
  {
    if (result.Content == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/contents/{result.Content.Id}", UriKind.Absolute);
      return Created(location, result.Content);
    }

    return Ok(result.Content);
  }
}
