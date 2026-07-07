using Microsoft.AspNetCore.Mvc;
using TADS_Web.Models.DB;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class AnnualController : BaseController
    {
        private readonly AnnualMeetingService _annualMeetingService;

        public AnnualController(AnnualMeetingService annualMeetingService)
        {
            _annualMeetingService = annualMeetingService;
        }

        public IActionResult Current()
        {
            return View();
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

            IQueryable<TbAnnualMeeting>? dataList = _annualMeetingService.GetItem(ref _message, id);
            TbAnnualMeeting? item = dataList?.FirstOrDefault(x => x.IsPublish);

            if (item == null)
                return RedirectToAction("History");

            return View(item);
        }
    }
}
