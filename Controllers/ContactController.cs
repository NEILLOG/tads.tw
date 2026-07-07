using Microsoft.AspNetCore.Mvc;
using TADS_Web.Service;

namespace TADS_Web.Controllers;

public class ContactController : Controller
{
    private readonly PageContentService _pageContentService;

    public ContactController(PageContentService pageContentService)
    {
        _pageContentService = pageContentService;
    }

    public IActionResult Index()
    {
        string msg = string.Empty;
        var item = _pageContentService.GetByCode(ref msg, "Contact")?.FirstOrDefault();
        return View(item);
    }
}
