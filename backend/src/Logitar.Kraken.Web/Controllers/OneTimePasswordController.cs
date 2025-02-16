using Logitar.Kraken.Contracts.Passwords;
using Logitar.Kraken.Core.Passwords.Commands;
using Logitar.Kraken.Core.Passwords.Queries;
using Logitar.Kraken.Web.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenAdmin)]
[Route("api/one-time-passwords")]
public class OneTimePasswordController : ControllerBase
{
  private readonly IMediator _mediator;

  public OneTimePasswordController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<OneTimePasswordModel>> CreateAsync([FromBody] CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    CreateOneTimePasswordCommand command = new(payload);
    OneTimePasswordModel oneTimePassword = await _mediator.Send(command, cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/one-time-passwords/{oneTimePassword.Id}", UriKind.Absolute);
    return Created(location, oneTimePassword);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<OneTimePasswordModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadOneTimePasswordQuery query = new(id);
    OneTimePasswordModel? oneTimePassword = await _mediator.Send(query, cancellationToken);
    return oneTimePassword == null ? NotFound() : Ok(oneTimePassword);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<OneTimePasswordModel>> ValidateAsync(Guid id, [FromBody] ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    ValidateOneTimePasswordCommand command = new(id, payload);
    OneTimePasswordModel? oneTimePassword = await _mediator.Send(command, cancellationToken);
    return oneTimePassword == null ? NotFound() : Ok(oneTimePassword);
  }
}
