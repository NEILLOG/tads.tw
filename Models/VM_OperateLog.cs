using Microsoft.AspNetCore.Mvc.Rendering;
using TADS_Web.Models.Base;
using TADS_Web.Models.Extend;

namespace TADS_Web.Models
{
    public class VM_OperateLog
    {
        public VM_OperateLogQueryParam Search { get; set; } = new VM_OperateLogQueryParam();
        public List<OperateLogExtend>? LogList { get; set; }
        public OperateLogExtend? LogItem { get; set; }

        /// <summary>功能下拉（由 DB 內既有的 Feature 值動態帶入）</summary>
        public List<SelectListItem> ddlFeature { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "全部功能", Value = "" }
        };

        public List<SelectListItem> ddlResult { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "全部結果", Value = "" },
            new SelectListItem() { Text = "成功", Value = "success" },
            new SelectListItem() { Text = "失敗", Value = "fail" }
        };
    }

    public class VM_OperateLogQueryParam : VM_BaseQueryParam
    {
        /// <summary>帳號 / 姓名關鍵字</summary>
        public string? sAccount { get; set; }

        /// <summary>功能</summary>
        public string? sFeature { get; set; }

        /// <summary>動作關鍵字</summary>
        public string? sAction { get; set; }

        /// <summary>結果：success / fail</summary>
        public string? sResult { get; set; }

        public string? sTime { get; set; }
        public string? eTime { get; set; }
    }
}
