using Microsoft.AspNetCore.Mvc.Rendering;
using TADS_Web.Models.Base;
using TADS_Web.Models.DB;

namespace TADS_Web.Models
{
    public class VM_AnnualMeeting
    {
        public VM_AnnualMeetingQueryParam Search { get; set; } = new VM_AnnualMeetingQueryParam();
        public List<TbAnnualMeeting>? AnnualMeetingList { get; set; }
        public TbAnnualMeeting? AnnualMeetingItem { get; set; }

        public List<SelectListItem> ddlPublish = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "上架", Value = "上架" },
            new SelectListItem() { Text = "下架", Value = "下架" }
        };

        public string? isPublish { get; set; }
    }

    public class VM_AnnualMeetingQueryParam : VM_BaseQueryParam
    {
        public string? sTitle { get; set; }
        public string? sPublish { get; set; }
    }
}
