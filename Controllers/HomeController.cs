using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TADS_Web.Models;
using TADS_Web.Models.Extend;
using TADS_Web.Service;

namespace TADS_Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MemberResearchService _memberResearchService;

    public HomeController(ILogger<HomeController> logger, MemberResearchService memberResearchService)
    {
        _logger = logger;
        _memberResearchService = memberResearchService;
    }

    public IActionResult Index()
    {
        // 以文會友（會員研究成果）最新六筆
        string message = string.Empty;
        IQueryable<MemberResearchExtend>? dataList = _memberResearchService.GetExtendList(ref message);

        ViewBag.MemberResearchLatest = dataList?
            .Where(x => x.Research.IsPublish)
            .OrderBy(x => x.Research.SortOrder)
            .ThenByDescending(x => x.Research.CreateDate)
            .Take(6)
            .ToList() ?? new List<MemberResearchExtend>();

        return View();
    }

    public IActionResult ResearchFindings()
    {
        return View();
    }

    public IActionResult News()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
