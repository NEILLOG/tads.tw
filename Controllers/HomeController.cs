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
    private readonly NewsService _newsService;
    private readonly BannerService _bannerService;

    public HomeController(ILogger<HomeController> logger, MemberResearchService memberResearchService, NewsService newsService, BannerService bannerService)
    {
        _logger = logger;
        _memberResearchService = memberResearchService;
        _newsService = newsService;
        _bannerService = bannerService;
    }

    public IActionResult Index()
    {
        string message = string.Empty;

        // 首頁橫幅（已上架，依排序）
        ViewBag.Banners = _bannerService.GetExtendList(ref message)?
            .Where(x => x.Banner.IsPublish && x.ImageUrl != null)
            .OrderBy(x => x.Banner.SortOrder)
            .ThenByDescending(x => x.Banner.CreateDate)
            .ToList() ?? new List<BannerExtend>();

        // 最新消息最新五筆
        IQueryable<NewsExtend>? newsList = _newsService.GetNewsExtendList(ref message);

        ViewBag.NewsLatest = newsList?
            .Where(x => x.News.IsPublish)
            .OrderByDescending(x => x.News.IsKeepTop)
            .ThenByDescending(x => x.News.DisplayDate)
            .Take(5)
            .ToList() ?? new List<NewsExtend>();

        // 以文會友（會員研究成果）最新六筆
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

    public IActionResult NewsList(string category)
    {
        string message = string.Empty;

        List<NewsExtend> publishedList = _newsService.GetNewsExtendList(ref message)?
            .Where(x => x.News.IsPublish)
            .OrderByDescending(x => x.News.IsKeepTop)
            .ThenByDescending(x => x.News.DisplayDate)
            .ToList() ?? new List<NewsExtend>();

        // 側邊欄：近期消息（最新五筆）
        ViewBag.RecentNews = publishedList.Take(5).ToList();

        // 側邊欄：分類統計
        ViewBag.CategoryCounts = publishedList
            .Where(x => !string.IsNullOrEmpty(x.News.Category))
            .GroupBy(x => x.News.Category)
            .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
            .ToList();

        ViewBag.CurrentCategory = category;

        List<NewsExtend> list = string.IsNullOrEmpty(category)
            ? publishedList
            : publishedList.Where(x => x.News.Category == category).ToList();

        return View(list);
    }

    public IActionResult News(string id, string category)
    {
        string message = string.Empty;

        List<NewsExtend> publishedList = _newsService.GetNewsExtendList(ref message)?
            .Where(x => x.News.IsPublish)
            .OrderByDescending(x => x.News.IsKeepTop)
            .ThenByDescending(x => x.News.DisplayDate)
            .ToList() ?? new List<NewsExtend>();

        // 側邊欄：近期消息（最新五筆）
        ViewBag.RecentNews = publishedList.Take(5).ToList();

        // 側邊欄：分類統計
        ViewBag.CategoryCounts = publishedList
            .Where(x => !string.IsNullOrEmpty(x.News.Category))
            .GroupBy(x => x.News.Category)
            .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
            .ToList();

        // 內文：優先指定 id，其次指定分類最新一筆，否則整體最新一筆
        NewsExtend? current;
        if (!string.IsNullOrEmpty(id))
            current = publishedList.FirstOrDefault(x => x.News.Id == id);
        else if (!string.IsNullOrEmpty(category))
            current = publishedList.FirstOrDefault(x => x.News.Category == category);
        else
            current = publishedList.FirstOrDefault();

        ViewBag.CurrentCategory = current?.News.Category;

        return View(current);
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
