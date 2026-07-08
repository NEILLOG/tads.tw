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

        /// <summary>後台上傳的附加檔案</summary>
        public IFormFile? AnnualFile { get; set; }

        /// <summary>欲刪除的附件 FileId 清單</summary>
        public List<string>? DelFileList { get; set; }

        /// <summary>現有附件下載路徑（編輯頁顯示用）</summary>
        public string? FileUrl { get; set; }

        /// <summary>現有附件原始檔名（編輯頁顯示用）</summary>
        public string? FileName { get; set; }
    }

    public class VM_AnnualMeetingQueryParam : VM_BaseQueryParam
    {
        public string? sTitle { get; set; }
        public string? sPublish { get; set; }
    }
}
