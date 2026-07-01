using TADS_Web.Models.DB;

namespace TADS_Web.Models.Extend
{
    public class NewsExtend
    {
        public TbNews News { get; set; } = null!;

        /// <summary>附件下載路徑（由 TbFileInfo.FilePath 組成）</summary>
        public string? FileUrl { get; set; }

        /// <summary>附件原始檔名（供前台下載顯示與檔名）</summary>
        public string? FileName { get; set; }
    }
}
