using Microsoft.AspNetCore.Mvc;

namespace ReusableServices.Mails.Controllers
{
  /// <inheritdoc />
  /// <summary>
  ///   The default home controller.
  /// </summary>
  [Route("mails/home")]
  [ApiController]
  public class HomeController : ControllerBase
  {
    /// <summary>
    ///   Get information about the service.
    /// </summary>
    /// <response code="200">Returns OK</response>
    [HttpGet]
    public ActionResult<string> Get()
    {
      return "Mails Service for the 1C Support system.";
    }
  }
}