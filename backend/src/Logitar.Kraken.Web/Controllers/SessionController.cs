using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core.Sessions.Commands;
using Logitar.Kraken.Core.Sessions.Queries;
using Logitar.Kraken.Web.Models.Session;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
  private readonly IMediator _mediator;

  public SessionController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<SessionModel>> CreateAsync([FromBody] CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    CreateSessionCommand command = new(payload);
    SessionModel session = await _mediator.Send(command, cancellationToken);
    Uri location = BuildLocation(session);
    return Created(location, session);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<SessionModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSessionQuery query = new(id);
    SessionModel? session = await _mediator.Send(query, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  [HttpPut("renew")]
  public async Task<ActionResult<SessionModel>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    RenewSessionCommand command = new(payload);
    SessionModel session = await _mediator.Send(command, cancellationToken);
    return Ok(session);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<SessionModel>>> SearchAsync([FromQuery] SearchSessionsParameters parameters, CancellationToken cancellationToken)
  {
    SearchSessionsPayload payload = parameters.ToPayload();
    SearchSessionsQuery query = new(payload);
    SearchResults<SessionModel> sessions = await _mediator.Send(query, cancellationToken);
    return Ok(sessions);
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<SessionModel>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SignInSessionCommand command = new(payload);
    SessionModel session = await _mediator.Send(command, cancellationToken);
    Uri location = BuildLocation(session);
    return Created(location, session);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<SessionModel>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutSessionCommand command = new(id);
    SessionModel? session = await _mediator.Send(command, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }
  private Uri BuildLocation(SessionModel session) => new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}", UriKind.Absolute);
}
