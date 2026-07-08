using Microsoft.AspNetCore.Mvc;
using TADS_Web.Models;
using TADS_Web.Models.Extend;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class AnnualController : BaseController
    {
        private readonly AnnualMeetingService _annualMeetingService;
        private readonly PageContentService _pageContentService;

        public AnnualController(AnnualMeetingService annualMeetingService, PageContentService pageContentService)
        {
            _annualMeetingService = annualMeetingService;
            _pageContentService = pageContentService;
        }

        public IActionResult Current()
        {
            // 三個區塊內容由後台「內頁管理」編輯，與首頁共用同一份資料
            VM_AnnualCurrentContent data = _pageContentService.GetAnnualCurrentContent(ref _message);
            data.ShowHistoryBlock = true;
            return View(data);
        }

        public IActionResult History()
        {
            return View();
        }

        /// <summary>歷屆年會內容頁（前台）</summary>
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("History");

            IQueryable<AnnualMeetingExtend>? dataList = _annualMeetingService.GetItemExtend(ref _message, id);
            AnnualMeetingExtend? item = dataList?.FirstOrDefault(x => x.AnnualMeeting.IsPublish);

            if (item == null)
                return RedirectToAction("History");

            return View(item);
        }
    }
}
