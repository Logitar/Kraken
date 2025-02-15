using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Configurations.Commands;
using Logitar.Kraken.Core.Configurations.Queries;
using Logitar.Kraken.Web.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenAdmin)]
[Route("api/configuration")]
public class ConfigurationController : ControllerBase
{
  private readonly IMediator _mediator;

  public ConfigurationController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet]
  public async Task<ActionResult<ConfigurationModel>> ReadAsync(CancellationToken cancellationToken)
  {
    ReadConfigurationQuery query = new();
    ConfigurationModel configuration = await _mediator.Send(query, cancellationToken);
    return Ok(configuration);
  }

  [HttpPut]
  public async Task<ActionResult<ConfigurationModel>> ReplaceAsync([FromBody] ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    ReplaceConfigurationCommand command = new(payload, version);
    ConfigurationModel configuration = await _mediator.Send(command, cancellationToken);
    return Ok(configuration);
  }

  [HttpPatch]
  public async Task<ActionResult<ConfigurationModel>> UpdateAsync([FromBody] UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    UpdateConfigurationCommand command = new(payload);
    ConfigurationModel configuration = await _mediator.Send(command, cancellationToken);
    return Ok(configuration);
  }
}
