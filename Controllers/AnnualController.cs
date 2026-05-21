using Microsoft.AspNetCore.Mvc;

namespace TADS_Web.Controllers
{
    public class AnnualController : BaseController
    {
        public IActionResult Current()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }
    }
}
