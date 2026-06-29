using Microsoft.AspNetCore.Mvc;

namespace TADS_Web.Controllers;

public class ContactController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
