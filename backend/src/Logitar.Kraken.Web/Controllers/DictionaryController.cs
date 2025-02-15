using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Dictionaries.Commands;
using Logitar.Kraken.Core.Dictionaries.Queries;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Models.Dictionary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenAdmin)]
[Route("api/dictionaries")]
public class DictionaryController : ControllerBase
{
  private readonly IMediator _mediator;

  public DictionaryController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<DictionaryModel>> CreateAsync([FromBody] CreateOrReplaceDictionaryPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceDictionaryCommand command = new(Id: null, payload, Version: null);
    CreateOrReplaceDictionaryResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<DictionaryModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteDictionaryCommand command = new(id);
    DictionaryModel? dictionary = await _mediator.Send(command, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<DictionaryModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadDictionaryQuery query = new(id, Language: null);
    DictionaryModel? dictionary = await _mediator.Send(query, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("language:{language}")]
  public async Task<ActionResult<DictionaryModel>> ReadAsync(string language, CancellationToken cancellationToken)
  {
    ReadDictionaryQuery query = new(Id: null, language);
    DictionaryModel? dictionary = await _mediator.Send(query, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<DictionaryModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceDictionaryCommand command = new(id, payload, version);
    CreateOrReplaceDictionaryResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<DictionaryModel>>> SearchAsync([FromQuery] SearchDictionariesParameters parameters, CancellationToken cancellationToken)
  {
    SearchDictionariesPayload payload = parameters.ToPayload();
    SearchDictionariesQuery query = new(payload);
    SearchResults<DictionaryModel> dictionaries = await _mediator.Send(query, cancellationToken);
    return Ok(dictionaries);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<DictionaryModel>> UpdateAsync(Guid id, [FromBody] UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    UpdateDictionaryCommand command = new(id, payload);
    DictionaryModel? dictionary = await _mediator.Send(command, cancellationToken);
    return dictionary == null ? NotFound() : Ok(dictionary);
  }

  private ActionResult<DictionaryModel> ToActionResult(CreateOrReplaceDictionaryResult result)
  {
    if (result.Dictionary == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/dictionaries/{result.Dictionary.Id}", UriKind.Absolute);
      return Created(location, result.Dictionary);
    }
    return Ok(result.Dictionary);
  }
}
