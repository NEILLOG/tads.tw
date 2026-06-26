using Microsoft.AspNetCore.Mvc;

namespace TADS_Web.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
