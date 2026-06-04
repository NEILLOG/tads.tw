using Microsoft.AspNetCore.Mvc;

namespace TADS_Web.Controllers
{
    public class SdatbackController : Controller
    {
        public IActionResult Sample()
        {
            ViewData["Title"] = "Sample Page";
            return View();
        }
    }
}
