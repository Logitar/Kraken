using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("kraken")]
public class KrakenController : Controller
{
  [HttpGet("{**anything}")]
  public ActionResult Index() => View();
}
