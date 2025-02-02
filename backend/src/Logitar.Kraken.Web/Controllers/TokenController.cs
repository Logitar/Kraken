using Logitar.Kraken.Contracts.Tokens;
using Logitar.Kraken.Core.Tokens.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Route("api/tokens")]
public class TokenController : ControllerBase
{
  private readonly IMediator _mediator;

  public TokenController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<CreatedTokenModel>> CreateAsync([FromBody] CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    CreateTokenCommand command = new(payload);
    CreatedTokenModel createdToken = await _mediator.Send(command, cancellationToken);
    return Ok(createdToken);
  }

  [HttpPut]
  public async Task<ActionResult<ValidatedTokenModel>> ValidateAsync([FromBody] ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    ValidateTokenCommand command = new(payload);
    ValidatedTokenModel validatedToken = await _mediator.Send(command, cancellationToken);
    return Ok(validatedToken);
  }
}
