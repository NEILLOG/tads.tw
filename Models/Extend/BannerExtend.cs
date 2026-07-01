using TADS_Web.Models.DB;

namespace TADS_Web.Models.Extend
{
    public class BannerExtend
    {
        public TbBanner Banner { get; set; } = null!;

        /// <summary>橫幅圖片顯示路徑（由 TbFileInfo.FilePath 組成）</summary>
        public string? ImageUrl { get; set; }
    }
}
