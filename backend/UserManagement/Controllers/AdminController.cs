using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerBase
  {

    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
      _logger = logger;
    }

    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public IActionResult GetMain(int locationId)
    {
      return Ok("Called Mainroute");
    }

    [HttpGet("{locationId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Get(int locationId)
    {
      return Ok($"Called subroute location={locationId}");
    }
  }
}