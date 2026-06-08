using Microsoft.AspNetCore.Mvc;

namespace TADS_Web.Controllers
{
    public class NewslettersController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
