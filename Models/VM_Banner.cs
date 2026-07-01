using Microsoft.AspNetCore.Mvc.Rendering;
using TADS_Web.Models.Base;
using TADS_Web.Models.Extend;

namespace TADS_Web.Models
{
    public class VM_Banner
    {
        public VM_BannerQueryParam Search { get; set; } = new VM_BannerQueryParam();
        public List<BannerExtend>? BannerList { get; set; }
        public BannerExtend? BannerItem { get; set; }

        public List<SelectListItem> ddlPublish = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "上架", Value = "上架" },
            new SelectListItem() { Text = "下架", Value = "下架" }
        };

        public IFormFile? BannerFile { get; set; }
        public List<string>? DelFileList { get; set; }
        public string? isPublish { get; set; }
    }

    public class VM_BannerQueryParam : VM_BaseQueryParam
    {
        public string? sTitle { get; set; }
        public string? sPublish { get; set; }
    }
}
