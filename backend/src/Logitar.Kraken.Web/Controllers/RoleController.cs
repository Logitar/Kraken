using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Roles.Commands;
using Logitar.Kraken.Core.Roles.Queries;
using Logitar.Kraken.Web.Models.Role;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController : ControllerBase
{
  private readonly IMediator _mediator;

  public RoleController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<RoleModel>> CreateAsync([FromBody] CreateOrReplaceRolePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRoleCommand command = new(Id: null, payload, Version: null);
    CreateOrReplaceRoleResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<RoleModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadRoleQuery command = new(id, UniqueName: null);
    RoleModel? role = await _mediator.Send(command, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<RoleModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ReadRoleQuery command = new(Id: null, uniqueName);
    RoleModel? role = await _mediator.Send(command, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<RoleModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRoleCommand command = new(id, payload, version);
    CreateOrReplaceRoleResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<RoleModel>>> SearchAsync([FromQuery] SearchRolesParameters parameters, CancellationToken cancellationToken)
  {
    SearchRolesPayload payload = parameters.ToPayload();
    SearchRolesQuery query = new(payload);
    SearchResults<RoleModel> roles = await _mediator.Send(query, cancellationToken);
    return Ok(roles);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<RoleModel>> UpdateAsync(Guid id, [FromBody] UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    UpdateRoleCommand command = new(id, payload);
    RoleModel? role = await _mediator.Send(command, cancellationToken);
    return role == null ? NotFound() : Ok(role);
  }

  private ActionResult<RoleModel> ToActionResult(CreateOrReplaceRoleResult result)
  {
    if (result.Role == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/roles/{result.Role.Id}", UriKind.Absolute);
      return Created(location, result.Role);
    }
    return Ok(result.Role);
  }
}
