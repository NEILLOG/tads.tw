using Microsoft.AspNetCore.Mvc;

namespace TADS_Web.Controllers;

public class DevelopmentDynamicsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
