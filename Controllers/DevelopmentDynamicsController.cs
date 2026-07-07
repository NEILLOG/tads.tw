using Microsoft.AspNetCore.Mvc;
using TADS_Web.Service;

namespace TADS_Web.Controllers;

public class DevelopmentDynamicsController : Controller
{
    private readonly PageContentService _pageContentService;

    public DevelopmentDynamicsController(PageContentService pageContentService)
    {
        _pageContentService = pageContentService;
    }

    public IActionResult Index()
    {
        string msg = string.Empty;
        var item = _pageContentService.GetByCode(ref msg, "DevelopmentDynamics")?.FirstOrDefault();
        return View(item);
    }
}
