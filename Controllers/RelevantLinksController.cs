using Microsoft.AspNetCore.Mvc;
using TADS_Web.Service;

namespace TADS_Web.Controllers;

public class RelevantLinksController : Controller
{
    private readonly PageContentService _pageContentService;

    public RelevantLinksController(PageContentService pageContentService)
    {
        _pageContentService = pageContentService;
    }

    public IActionResult Index()
    {
        string msg = string.Empty;
        var item = _pageContentService.GetByCode(ref msg, "RelevantLinks")?.FirstOrDefault();
        return View(item);
    }
}
