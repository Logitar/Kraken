using Logitar.Kraken.Models.Index;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Controllers;

[ApiController]
[Route("api")]
public class IndexController : ControllerBase
{
  [HttpGet]
  public ActionResult<ApiVersion> Get() => Ok(ApiVersion.Current);
}
