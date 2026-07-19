using TADS_Web.Models.DB;

namespace TADS_Web.Models
{
    public class VM_AboutContent
    {
        public TbAboutContent AboutItem { get; set; } = new TbAboutContent();
        public List<TbAboutPricing> PricingList { get; set; } = new List<TbAboutPricing>();

        // 由 FileId 解析出的檔案 URL（前台顯示 / 後台預覽用）
        public string? IntroImageUrl { get; set; }
        public string? OrgChartUrl { get; set; }
        public string? ConstitutionPdfUrl { get; set; }
        public string? MembershipFormUrl { get; set; }
    }
}
