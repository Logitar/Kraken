using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Localization.Commands;
using Logitar.Kraken.Core.Localization.Queries;
using Logitar.Kraken.Web.Constants;
using Logitar.Kraken.Web.Models.Language;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenAdmin)]
[Route("api/languages")]
public class LanguageController : ControllerBase
{
  private readonly IMediator _mediator;

  public LanguageController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<LanguageModel>> CreateAsync([FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _mediator.Send(new CreateOrReplaceLanguageCommand(Id: null, payload, Version: null), cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<LanguageModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _mediator.Send(new DeleteLanguageCommand(id), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LanguageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _mediator.Send(new ReadLanguageQuery(id, Locale: null, IsDefault: false), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet("locale:{locale}")]
  public async Task<ActionResult<LanguageModel>> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _mediator.Send(new ReadLanguageQuery(Id: null, locale, IsDefault: false), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet("default")]
  public async Task<ActionResult<LanguageModel>> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    LanguageModel? language = await _mediator.Send(new ReadLanguageQuery(Id: null, Locale: null, IsDefault: true), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LanguageModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLanguagePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _mediator.Send(new CreateOrReplaceLanguageCommand(id, payload, version), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LanguageModel>>> SearchAsync([FromQuery] SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<LanguageModel> languages = await _mediator.Send(new SearchLanguagesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(languages);
  }

  [HttpPatch("{id}/default")]
  public async Task<ActionResult<LanguageModel>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _mediator.Send(new SetDefaultLanguageCommand(id), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<LanguageModel>> UpdateAsync(Guid id, [FromBody] UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _mediator.Send(new UpdateLanguageCommand(id, payload), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  private ActionResult<LanguageModel> ToActionResult(CreateOrReplaceLanguageResult result)
  {
    if (result.Language == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/languages/{result.Language.Id}", UriKind.Absolute);
      return Created(location, result.Language);
    }
    return Ok(result.Language);
  }
}
