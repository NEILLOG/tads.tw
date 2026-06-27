using Microsoft.AspNetCore.Mvc;
using TADS_Web.Models;
using TADS_Web.Models.Extend;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class MemberResearchController : BaseController
    {
        private readonly MemberResearchService _memberResearchService;

        public MemberResearchController(MemberResearchService memberResearchService)
        {
            _memberResearchService = memberResearchService;
        }

        public IActionResult List()
        {
            VM_MemberResearch data = new VM_MemberResearch();

            IQueryable<MemberResearchExtend>? dataList = _memberResearchService.GetExtendList(ref _message);
            if (dataList != null)
            {
                data.ResearchList = dataList
                    .Where(x => x.Research.IsPublish)
                    .OrderBy(x => x.Research.SortOrder)
                    .ThenByDescending(x => x.Research.CreateDate)
                    .ToList();
            }

            return View(data);
        }

        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("List");

            VM_MemberResearch data = new VM_MemberResearch();

            IQueryable<MemberResearchExtend>? dataList = _memberResearchService.GetExtendItem(ref _message, id);
            data.ResearchItem = dataList?.FirstOrDefault(x => x.Research.IsPublish);

            if (data.ResearchItem == null)
                return RedirectToAction("List");

            return View(data);
        }
    }
}
