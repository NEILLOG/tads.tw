using Microsoft.AspNetCore.Mvc;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class NewslettersController : BaseController
    {
        private readonly PageContentService _pageContentService;

        public NewslettersController(PageContentService pageContentService)
        {
            _pageContentService = pageContentService;
        }

        public IActionResult Index()
        {
            var item = _pageContentService.GetByCode(ref _message, "Newsletters")?.FirstOrDefault();
            return View(item);
        }
    }
}
