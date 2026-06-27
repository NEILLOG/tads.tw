using Microsoft.AspNetCore.Mvc;

namespace TADS_Web.Controllers;

public class RelevantLinksController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
