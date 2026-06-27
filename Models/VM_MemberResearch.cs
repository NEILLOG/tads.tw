using Microsoft.AspNetCore.Mvc.Rendering;
using TADS_Web.Models.Base;
using TADS_Web.Models.Extend;

namespace TADS_Web.Models
{
    public class VM_MemberResearch
    {
        public VM_MemberResearchQueryParam Search { get; set; } = new VM_MemberResearchQueryParam();
        public List<MemberResearchExtend>? ResearchList { get; set; }
        public MemberResearchExtend? ResearchItem { get; set; }

        public List<SelectListItem> ddlPublish = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "上架", Value = "上架" },
            new SelectListItem() { Text = "下架", Value = "下架" }
        };

        public IFormFile? CoverFile { get; set; }
        public List<string>? DelFileList { get; set; }
        public string? isPublish { get; set; }
    }

    public class VM_MemberResearchQueryParam : VM_BaseQueryParam
    {
        public string? sTitle { get; set; }
        public string? sPublish { get; set; }
    }
}
