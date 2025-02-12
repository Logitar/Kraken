using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Realms.Commands;
using Logitar.Kraken.Core.Realms.Queries;
using Logitar.Kraken.Web.Models.Realm;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
//[Authorize] // TODO(fpion): Authorization
[Route("api/realms")]
public class RealmController : ControllerBase
{
  private readonly IMediator _mediator;

  public RealmController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<RealmModel>> CreateAsync([FromBody] CreateOrReplaceRealmPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealmCommand command = new(Id: null, payload, Version: null);
    CreateOrReplaceRealmResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<RealmModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteRealmCommand command = new(id);
    RealmModel? realm = await _mediator.Send(command, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<RealmModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadRealmQuery query = new(id, UniqueSlug: null);
    RealmModel? realm = await _mediator.Send(query, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpGet("slug:{uniqueSlug}")]
  public async Task<ActionResult<RealmModel>> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    ReadRealmQuery query = new(Id: null, uniqueSlug);
    RealmModel? realm = await _mediator.Send(query, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<RealmModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealmCommand command = new(id, payload, version);
    CreateOrReplaceRealmResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<RealmModel>>> SearchAsync([FromQuery] SearchRealmsParameters parameters, CancellationToken cancellationToken)
  {
    SearchRealmsPayload payload = parameters.ToPayload();
    SearchRealmsQuery query = new(payload);
    SearchResults<RealmModel> realms = await _mediator.Send(query, cancellationToken);
    return Ok(realms);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<RealmModel>> UpdateAsync(Guid id, [FromBody] UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    UpdateRealmCommand command = new(id, payload);
    RealmModel? realm = await _mediator.Send(command, cancellationToken);
    return realm == null ? NotFound() : Ok(realm);
  }

  private ActionResult<RealmModel> ToActionResult(CreateOrReplaceRealmResult result)
  {
    if (result.Realm == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/realms/{result.Realm.Id}", UriKind.Absolute);
      return Created(location, result.Realm);
    }

    return Ok(result.Realm);
  }
}
