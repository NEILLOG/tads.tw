using Microsoft.AspNetCore.Mvc.Rendering;
using TADS_Web.Models.Base;
using TADS_Web.Models.Extend;

namespace TADS_Web.Models
{
    public class VM_News
    {
        public VM_NewsQueryParam Search { get; set; } = new VM_NewsQueryParam();
        public List<NewsExtend>? NewsExtendList { get; set; }
        public NewsExtend? NewsExtendItem { get; set; }
        public List<string>? SortList { get; set; }

        public List<SelectListItem> ddlCategory = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "請選擇", Value = "" },
            new SelectListItem() { Text = "計畫消息", Value = "計畫消息" },
            new SelectListItem() { Text = "其他消息", Value = "其他消息" }
        };

        public List<SelectListItem> ddlPublish = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "上架", Value = "上架" },
            new SelectListItem() { Text = "下架", Value = "下架" }
        };

        public IFormFile? NewsFile { get; set; }
        public List<string>? DelFileList { get; set; }
        public string? isPublish { get; set; }
        public List<string> TopId { get; set; } = new List<string>();
    }

    public class VM_NewsQueryParam : VM_BaseQueryParam
    {
        public string? Keyword { get; set; }
        public string? sCategory { get; set; }
        public string? sTitle { get; set; }
        public string? sTime { get; set; }
        public string? eTime { get; set; }
        public string? sPublish { get; set; }
    }
}
