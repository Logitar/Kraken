using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Contents.Queries;
using Logitar.Kraken.Web.Models.PublishedContent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/published/contents")]
public class PublishedContentController : ControllerBase
{
  private readonly IMediator _mediator;

  public PublishedContentController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PublishedContent>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishedContent? content = await _mediator.Send(new ReadPublishedContentQuery(ContentId: null, ContentUid: id, Key: null), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpGet("types/{contentType}/unique-name:{uniqueName}")]
  public async Task<ActionResult<PublishedContent>> ReadAsync(string contentType, string uniqueName, string? language, CancellationToken cancellationToken)
  {
    PublishedContentKey key = new(contentType, language, uniqueName);
    PublishedContent? content = await _mediator.Send(new ReadPublishedContentQuery(ContentId: null, ContentUid: null, key), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<PublishedContentLocale>>> SearchAsync([FromQuery] SearchPublishedContentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<PublishedContentLocale> contents = await _mediator.Send(new SearchPublishedContentsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contents);
  }
}
