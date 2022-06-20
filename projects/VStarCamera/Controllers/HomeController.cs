using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VStarCameraZone.Entities;

namespace VStarCameraZone.Controllers;

/// <summary>
/// Main controller.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IOptions<CameraCredentials> options)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
