using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.ApiKeys.Commands;
using Logitar.Kraken.Core.ApiKeys.Queries;
using Logitar.Kraken.Web.Models.ApiKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/keys")]
public class ApiKeyController : ControllerBase
{
  private readonly IMediator _mediator;

  public ApiKeyController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPatch("authenticate")]
  public async Task<ActionResult<ApiKeyModel>> AuthenticateAsync([FromBody] AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    AuthenticateApiKeyCommand command = new(payload);
    ApiKeyModel apiKey = await _mediator.Send(command, cancellationToken);
    return Ok(apiKey);
  }

  [HttpPost]
  public async Task<ActionResult<ApiKeyModel>> CreateAsync([FromBody] CreateOrReplaceApiKeyPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceApiKeyCommand command = new(Id: null, payload, Version: null);
    CreateOrReplaceApiKeyResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiKeyModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadApiKeyQuery command = new(id);
    ApiKeyModel? apiKey = await _mediator.Send(command, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ApiKeyModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceApiKeyCommand command = new(id, payload, version);
    CreateOrReplaceApiKeyResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ApiKeyModel>>> SearchAsync([FromQuery] SearchApiKeysParameters parameters, CancellationToken cancellationToken)
  {
    SearchApiKeysPayload payload = parameters.ToPayload();
    SearchApiKeysQuery query = new(payload);
    SearchResults<ApiKeyModel> apiKeys = await _mediator.Send(query, cancellationToken);
    return Ok(apiKeys);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ApiKeyModel>> UpdateAsync(Guid id, [FromBody] UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    UpdateApiKeyCommand command = new(id, payload);
    ApiKeyModel? apiKey = await _mediator.Send(command, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  private ActionResult<ApiKeyModel> ToActionResult(CreateOrReplaceApiKeyResult result)
  {
    if (result.ApiKey == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri uri = new($"{Request.Scheme}://{Request.Host}/api/keys/{result.ApiKey.Id}", UriKind.Absolute);
      return Created(uri, result.ApiKey);
    }
    return Ok(result.ApiKey);
  }
}
