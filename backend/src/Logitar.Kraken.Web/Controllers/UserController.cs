using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Users.Commands;
using Logitar.Kraken.Core.Users.Queries;
using Logitar.Kraken.Web.Models.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly IMediator _mediator;

  public UserController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPatch("authenticate")]
  public async Task<ActionResult<UserModel>> AuthenticateAsync([FromBody] AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    AuthenticateUserCommand command = new(payload);
    UserModel user = await _mediator.Send(command, cancellationToken);
    return Ok(user);
  }

  [HttpPost]
  public async Task<ActionResult<UserModel>> CreateAsync([FromBody] CreateOrReplaceUserPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceUserCommand command = new(Id: null, payload, Version: null);
    CreateOrReplaceUserResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<UserModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadUserQuery query = new(id, UniqueName: null, Identifier: null);
    UserModel? user = await _mediator.Send(query, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("identifier/key:{key}/value:{value}")]
  public async Task<ActionResult<UserModel>> ReadAsync(string key, string value, CancellationToken cancellationToken)
  {
    ReadUserQuery query = new(Id: null, UniqueName: null, new CustomIdentifierModel(key, value));
    UserModel? user = await _mediator.Send(query, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<UserModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ReadUserQuery query = new(Id: null, uniqueName, Identifier: null);
    UserModel? user = await _mediator.Send(query, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpDelete("{id}/identifiers/key:{key}")]
  public async Task<ActionResult<UserModel>> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    RemoveUserIdentifierCommand command = new(id, key);
    UserModel? user = await _mediator.Send(command, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<UserModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceUserCommand command = new(id, payload, version);
    CreateOrReplaceUserResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}/password/reset")]
  public async Task<ActionResult<UserModel>> ResetPasswordAsync(Guid id, [FromBody] ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    ResetUserPasswordCommand command = new(id, payload);
    UserModel? user = await _mediator.Send(command, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}/identifiers/key:{key}")]
  public async Task<ActionResult<UserModel>> SaveIdentifierAsync(Guid id, string key, [FromBody] SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    SaveUserIdentifierCommand command = new(id, key, payload);
    UserModel? user = await _mediator.Send(command, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<UserModel>>> SearchAsync([FromQuery] SearchUsersParameters parameters, CancellationToken cancellationToken)
  {
    SearchUsersPayload payload = parameters.ToPayload();
    SearchUsersQuery query = new(payload);
    SearchResults<UserModel> users = await _mediator.Send(query, cancellationToken);
    return Ok(users);
  }

  [HttpPut("{id}/sign/out")]
  public async Task<ActionResult<UserModel>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutUserCommand command = new(id);
    UserModel? user = await _mediator.Send(command, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<UserModel>> UpdateAsync(Guid id, [FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    UpdateUserCommand command = new(id, payload);
    UserModel? user = await _mediator.Send(command, cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  private ActionResult<UserModel> ToActionResult(CreateOrReplaceUserResult result)
  {
    if (result.User == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/users/{result.User.Id}", UriKind.Absolute);
      return Created(location, result.User);
    }
    return Ok(result.User);
  }
}
